using System.Collections;
using System.Runtime.CompilerServices;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Systems
{
    public class GameFieldWaveAlgorithmSystem : IEcsRun
    {
        private readonly ICoroutineRunner _coroutineRunner;
        [EcsInject] private EcsWorld _world;

        public GameFieldWaveAlgorithmSystem(ICoroutineRunner coroutineRunner) =>
            _coroutineRunner = coroutineRunner;

        private class GenerationAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameFieldWaveAlgorithmTag))]
            [IncImplicit(typeof(GameFieldGenerateRequest))]
            [Inc] public readonly EcsPool<Wave> Waves;

            [Inc] public readonly EcsPool<TargetEntity> Targets;
        }

        private class DestructionAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameFieldWaveAlgorithmTag))]
            [IncImplicit(typeof(GameFieldDestructRequest))]
            [Inc] public readonly EcsPool<Wave> Waves;

            [Inc] public readonly EcsPool<TargetEntity> Targets;
        }

        private class TargetLevelAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameField> GameFields;

            [Opt] public readonly EcsTagPool<GameFieldGeneratedEvent> GameFieldGeneratedEvent;
            [Opt] public readonly EcsTagPool<GameFieldDestructedEvent> GameFieldDestructedEvent;
            [Opt] public readonly EcsTagPool<GameFieldGeneratedMarker> GameFieldGeneratedMarker;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out GenerationAspect aspect))
            {
                _coroutineRunner
                    .StartCoroutine(Generate(
                        generationAspect: aspect,
                        levelAspect: _world.GetAspect<TargetLevelAspect>(),
                        algorithm: entity));
            }

            foreach (int entity in _world.Where(out DestructionAspect aspect))
            {
                _coroutineRunner
                    .StartCoroutine(Destruct(
                        destructionAspect: aspect,
                        levelAspect: _world.GetAspect<TargetLevelAspect>(),
                        algorithm: entity
                    ));
            }
        }

        private IEnumerator Generate(GenerationAspect generationAspect, TargetLevelAspect levelAspect, int algorithm)
        {
            if (!generationAspect.Targets.Read(algorithm).Value.TryGetID(out int levelID) ||
                !levelAspect.IsMatches(levelID))
                yield break;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            ref GameField GameField() =>
                ref levelAspect.GameFields.Get(levelID);

            float3 waveOrigin = generationAspect.Waves.Get(algorithm).WaveOrigin = new float3(
                GameField().Size / 2f * (GameField().CellSize + GameField().Offset) + GameField().OriginPosition.x,
                0,
                GameField().Size / 2f * (GameField().CellSize + GameField().Offset) + GameField().OriginPosition.z);

            int counter = 0;

            for (int x = 0; x < GameField().Size; x++)
            {
                for (int z = 0; z < GameField().Size; z++)
                {
                    if (x >= GameField().EdgeSize && x < GameField().EdgeSize + GameField().CenterSize ||
                        z >= GameField().EdgeSize && z < GameField().EdgeSize + GameField().CenterSize)
                    {
                        float3 position = GridUtils.GetWorldPosition(
                            coordinates: new int2(x, z),
                            gameField: in GameField());

                        float delay = math.distance(position, waveOrigin) *
                                      generationAspect.Waves.Read(algorithm).Speed;

                        yield return new WaitForSeconds(delay);

                        GameObject view = Object.Instantiate(
                            original: GameField().CellPrefab,
                            position: position,
                            rotation: Quaternion.identity);

                        view.transform.localScale = new Vector3(
                            GameField().CellSize,
                            view.transform.localScale.y,
                            GameField().CellSize);

                        GameField().Cells[counter++] = new GameField.Cell
                        {
                            View = view,
                            CellPosition = new float2(x, z),
                            WorldPosition = position
                        };

                        int tile = _world.NewEntity();

                        _world.GetPool<TileGeneratedEvent>().Add(tile);
                        _world.GetPool<TargetEntity>().Add(tile).Value = _world.GetEntityLong(levelID);
                        _world.GetPool<DeleteEntityCommand>().Add(tile);
                    }
                }
            }

            Debug.Log("End");

            levelAspect.GameFieldGeneratedEvent.Add(levelID);
            levelAspect.GameFieldGeneratedEvent.Add(algorithm);
            levelAspect.GameFieldGeneratedMarker.Add(levelID);
        }

        private IEnumerator Destruct(DestructionAspect destructionAspect, TargetLevelAspect levelAspect, int algorithm)
        {
            if (!destructionAspect.Targets.Read(algorithm).Value.TryGetID(out int levelID) ||
                !levelAspect.IsMatches(levelID))
                yield break;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            ref GameField GameField() =>
                ref levelAspect.GameFields.Get(levelID);

            levelAspect.GameFieldGeneratedMarker.Del(levelID);
            
            foreach (GameField.Cell cell in GameField().Cells)
            {
                float delay = math.distance(cell.WorldPosition, destructionAspect.Waves.Read(algorithm).WaveOrigin) *
                              destructionAspect.Waves.Read(algorithm).Speed;

                yield return new WaitForSeconds(delay);

                Object.Destroy(cell.View);
            }

            levelAspect.GameFieldDestructedEvent.Add(levelID);
            levelAspect.GameFieldDestructedEvent.Add(algorithm);
        }
    }
}