using System.Collections;
using System.Runtime.CompilerServices;
using _Project.Scripts.DragonAPI;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Extensions;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Utils;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Gameplay.Features.GameFieldFeature.Systems
{
    public class GameFieldWaveAlgorithmSystem : IEcsInit, IEcsRun
    {
        [EcsInject] private EcsWorld _world;
        private DragonCoroutineRunner _dragonCoroutineRunner;

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

        private class GameFieldAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameField> GameFields;

            [Opt] public readonly EcsTagPool<GameFieldGeneratedEvent> GameFieldGeneratedEvent;
            [Opt] public readonly EcsTagPool<GameFieldDestructedEvent> GameFieldDestructedEvent;
            [Opt] public readonly EcsTagPool<GameFieldGeneratedMarker> GameFieldGeneratedMarker;
            [Opt] public readonly EcsTagPool<GameFieldDestructedMarker> GameFieldDestructedMarker;
            [Opt] public readonly EcsPool<GameFieldGeneratedByAlgorithm> GameFieldGeneratedByAlgorithm;
        }

        public void Init() =>
            _dragonCoroutineRunner = DragonAPI.DragonAPI.CreateCoroutineRunner();

        public void Run()
        {
            _dragonCoroutineRunner.Tick();

            foreach (int entity in _world.Where(out GenerationAspect aspect))
            {
                _dragonCoroutineRunner.StartCoroutine(Generate(
                    generationAspect: aspect,
                    gameFieldAspect: _world.GetAspect<GameFieldAspect>(),
                    algorithm: entity));
            }

            foreach (int entity in _world.Where(out DestructionAspect aspect))
            {
                _dragonCoroutineRunner.StartCoroutine(Destruct(
                    destructionAspect: aspect,
                    gameFieldAspect: _world.GetAspect<GameFieldAspect>(),
                    algorithm: entity
                ));
            }
        }

        private IEnumerator Generate(GenerationAspect generationAspect, GameFieldAspect gameFieldAspect, int algorithm)
        {
            if (!generationAspect.Targets.Read(algorithm).Value.TryGetID(out int gameFieldID) ||
                !gameFieldAspect.IsMatches(gameFieldID))
                yield break;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            ref GameField GameField() =>
                ref gameFieldAspect.GameFields.Get(gameFieldID);

            gameFieldAspect.GameFieldDestructedMarker.TryDel(gameFieldID);

            float3 waveOrigin = generationAspect.Waves.Get(algorithm).WaveOrigin = new float3(
                GameField().Size / 2f * (GameField().CellSize + GameField().Offset) + GameField().OriginPosition.x,
                0,
                GameField().Size / 2f * (GameField().CellSize + GameField().Offset) + GameField().OriginPosition.z);

            generationAspect.Waves.Get(algorithm).SpeedFactor = generationAspect.Waves.Read(algorithm).BaseSpeedFactor *
                                                                ((float)GameField().BaseSize / GameField().Size);

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
                            grid: GameField().ToGrid());

                        float delay = math.distance(position, waveOrigin) *
                                      generationAspect.Waves.Get(algorithm).SpeedFactor;

                        GameObject view = Object.Instantiate(
                            original: GameField().CellPrefab,
                            position: position,
                            rotation: Quaternion.identity);

                        view.transform.localScale = new Vector3(
                            GameField().CellSize,
                            GameField().CellScaleY,
                            GameField().CellSize);

                        GameField().Cells[counter++] = new GameField.Cell
                        {
                            View = view,
                            WorldPosition = position
                        };

                        int tile = _world.NewEntity();

                        _world.GetPool<TileGeneratedEvent>().Add(tile);
                        _world.GetPool<TargetEntity>().Add(tile).Value = _world.GetEntityLong(gameFieldID);

                        yield return new DragonAPI.YieldInstructions.DragonAPI.WaitForSeconds(delay);
                    }
                }
            }

            gameFieldAspect.GameFieldGeneratedByAlgorithm.TryAddOrGet(gameFieldID).Value =
                algorithm.ToEntityLong(_world);

            gameFieldAspect.GameFieldGeneratedMarker.Add(gameFieldID);
            gameFieldAspect.GameFieldGeneratedEvent.Add(gameFieldID);
        }

        private static IEnumerator Destruct(DestructionAspect destructionAspect, GameFieldAspect gameFieldAspect,
            int algorithm)
        {
            if (!destructionAspect.Targets.Read(algorithm).Value.TryGetID(out int gameFieldID) ||
                !gameFieldAspect.IsMatches(gameFieldID))
                yield break;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            ref GameField GameField() =>
                ref gameFieldAspect.GameFields.Get(gameFieldID);

            gameFieldAspect.GameFieldGeneratedMarker.Del(gameFieldID);

            foreach (GameField.Cell cell in GameField().Cells)
            {
                float delay = math.distance(cell.WorldPosition, destructionAspect.Waves.Read(algorithm).WaveOrigin) *
                              destructionAspect.Waves.Read(algorithm).SpeedFactor;

                yield return new DragonAPI.YieldInstructions.DragonAPI.WaitForSeconds(delay);

                Object.Destroy(cell.View);
            }

            gameFieldAspect.GameFieldDestructedMarker.Add(gameFieldID);

            gameFieldAspect.GameFieldGeneratedByAlgorithm.Del(gameFieldID);
            gameFieldAspect.GameFieldDestructedEvent.Add(gameFieldID);
        }
    }
}