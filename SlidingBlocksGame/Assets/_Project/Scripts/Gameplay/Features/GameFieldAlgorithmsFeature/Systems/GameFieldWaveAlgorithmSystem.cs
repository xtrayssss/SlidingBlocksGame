using System;
using System.Collections;
using System.Runtime.CompilerServices;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Systems
{
    public class GameFieldWaveAlgorithmSystem : IEcsRun
    {
        [EcsInject] private EcsWorld _world;

        private class GenerationAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameFieldWaveAlgorithmTag))]
            [IncImplicit(typeof(GameFieldGenerateRequest))]
            [Inc] public readonly EcsPool<WaveAlgorithm> Waves;

            [Inc] public readonly EcsPool<TargetEntity> Targets;
        }

        private class DestructionAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameFieldWaveAlgorithmTag))]
            [IncImplicit(typeof(GameFieldDestructRequest))]
            [Inc] public readonly EcsPool<WaveAlgorithm> Waves;

            [Inc] public readonly EcsPool<TargetEntity> Targets;
        }

        private class TargetLevelAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameField> GameFields;

            [Opt] public readonly EcsTagPool<GameFieldGeneratedEvent> GameFieldGenerated;
            [Opt] public readonly EcsTagPool<GameFieldDestructedEvent> GameFieldDestructed;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out GenerationAspect aspect))
            {
                Object.FindAnyObjectByType<MonoBehaviour>()
                    .StartCoroutine(Generate(
                        generationAspect: aspect,
                        levelAspect: _world.GetAspect<TargetLevelAspect>(),
                        algorithm: entity));
            }

            foreach (int entity in _world.Where(out DestructionAspect aspect))
            {
                Object.FindAnyObjectByType<MonoBehaviour>()
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

            float3 waveOrigin = new float3(
                GameField().Size / 2f * (GameField().CellSize + GameField().Offset) + GameField().OriginPosition.x,
                0, GameField().Size / 2f * (GameField().CellSize + GameField().Offset) + GameField().OriginPosition.z);

            int counter = 0;

            for (int x = 0; x < GameField().Size; x++)
            {
                for (int z = 0; z < GameField().Size; z++)
                {
                    if (x >= GameField().EdgeSize && x < GameField().EdgeSize + GameField().CenterSize ||
                        z >= GameField().EdgeSize && z < GameField().EdgeSize + GameField().CenterSize)
                    {
                        float3 position = new float3(
                            x * (GameField().CellSize + GameField().Offset) + GameField().OriginPosition.x, 0,
                            z * (GameField().CellSize + GameField().Offset) + GameField().OriginPosition.z);

                        float delay = math.distance(position, waveOrigin) *
                                      generationAspect.Waves.Read(algorithm).Speed;

                        yield return new WaitForSeconds(delay);

                        GameObject view = Object.Instantiate(GameField().CellPrefab, position, Quaternion.identity);

                        view.transform.localScale  = new Vector3(GameField().CellSize, view.transform.localScale.y,
                            GameField().CellSize);
                        
                        GameField().Cells[counter++] = new GameField.Cell
                        {
                            View = view,
                            CellPosition = new float2(x, z),
                            WorldPosition = position
                        };

                        int @event = _world.NewEntity();
                        
                        _world.GetPool<TileGeneratedEvent>().Add(@event);
                        _world.GetPool<TargetEntity>().Add(@event).Value = _world.GetEntityLong(levelID);
                    }
                }
            }

            levelAspect.GameFieldGenerated.Add(levelID);
            levelAspect.GameFieldGenerated.Add(algorithm);
        }

        private IEnumerator Destruct(DestructionAspect destructionAspect, TargetLevelAspect levelAspect, int algorithm)
        {
            if (!destructionAspect.Targets.Read(algorithm).Value.TryGetID(out int levelID) ||
                !levelAspect.IsMatches(levelID))
                yield break;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            ref GameField GameField() =>
                ref levelAspect.GameFields.Get(levelID);

            float3 waveOrigin = new float3(
                GameField().Size / 2f * (GameField().CellSize + GameField().Offset) + GameField().OriginPosition.x,
                0, GameField().Size / 2f * (GameField().CellSize + GameField().Offset) + GameField().OriginPosition.z);

            foreach (GameField.Cell cell in GameField().Cells)
            {
                float3 position = new float3(
                    cell.CellPosition.x * (GameField().CellSize + GameField().Offset) + GameField().OriginPosition.x, 0,
                    cell.CellPosition.y * (GameField().CellSize + GameField().Offset) + GameField().OriginPosition.z);

                float delay = math.distance(position, waveOrigin) * destructionAspect.Waves.Read(algorithm).Speed;

                yield return new WaitForSeconds(delay);

                Object.Destroy(cell.View);
            }

            levelAspect.GameFieldDestructed.Add(levelID);
            levelAspect.GameFieldDestructed.Add(algorithm);
        }
    }
}