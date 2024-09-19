using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
    public class GameFieldGrowthWaveAlgorithmSystem : IEcsRun
    {
        [EcsInject] private EcsWorld _world;

        private readonly ICoroutineRunner _coroutineRunner;

        public GameFieldGrowthWaveAlgorithmSystem(ICoroutineRunner coroutineRunner) =>
            _coroutineRunner = coroutineRunner;

        private class GenerationAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameFieldGrowthWaveAlgorithmTag))]
            [IncImplicit(typeof(GameFieldGenerateRequest))]
            [Inc] public readonly EcsPool<GrowthWave> GrowthWaves;

            [Inc] public readonly EcsPool<TargetEntity> Targets;
        }

        private class DestructionAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameFieldGrowthWaveAlgorithmTag))]
            [IncImplicit(typeof(GameFieldDestructRequest))]
            [Inc] public readonly EcsPool<GrowthWave> GrowthWaves;

            [Inc] public readonly EcsPool<TargetEntity> Targets;
        }

        private class TargetLevelAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameField> GameFields;

            [Opt] public readonly EcsTagPool<GameFieldGeneratedEvent> GameFieldGenerated;
            [Opt] public readonly EcsTagPool<GameFieldDestructedEvent> GameFieldDestructed;
            [Opt] public readonly EcsTagPool<GameFieldGeneratedMarker> GameFieldGeneratedMarker;
            [Opt] public readonly EcsTagPool<GameFieldDestructedMarker> GameFieldDestructedMarker;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out GenerationAspect aspect))
            {
                Generate(
                    generationAspect: aspect,
                    levelAspect: _world.GetAspect<TargetLevelAspect>(),
                    algorithm: entity);
            }

            foreach (int entity in _world.Where(out DestructionAspect aspect))
            {
                Destruct(
                    destructionAspect: aspect,
                    levelAspect: _world.GetAspect<TargetLevelAspect>(),
                    algorithm: entity);
            }
        }

        private async void Generate(GenerationAspect generationAspect, TargetLevelAspect levelAspect, int algorithm)
        {
            if (!generationAspect.Targets.Read(algorithm).Value.TryGetID(out int levelID) ||
                !levelAspect.IsMatches(levelID))
                return;

            int counter = 0;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            ref GameField GameField() =>
                ref levelAspect.GameFields.Get(levelID);
            
            levelAspect.GameFieldDestructedMarker.TryDel(levelID);
            
            generationAspect.GrowthWaves.Get(algorithm).GrowthTasks ??= new Task[GameField().CellsCount];

            for (int x = 0; x < GameField().Size; x++)
            {
                for (int z = 0; z < GameField().Size; z++)
                {
                    if (x >= GameField().EdgeSize && x < GameField().EdgeSize + GameField().CenterSize ||
                        z >= GameField().EdgeSize && z < GameField().EdgeSize + GameField().CenterSize)
                    {
                        float3 position = Utils.GridUtils.GetWorldPosition(new int2(x, z), in GameField());

                        GameObject view = Object.Instantiate(
                            original: GameField().CellPrefab,
                            position: position,
                            rotation: Quaternion.identity);

                        view.transform.localScale = float3.zero;

                        generationAspect.GrowthWaves.Get(algorithm).GrowthTasks[counter] =
                            GrowTile(view, generationAspect, algorithm, levelID, levelAspect)
                                .ContinueWith(
                                    _ =>
                                    {
                                        int tile = _world.NewEntity();

                                        _world.GetPool<TileGeneratedEvent>().Add(tile);
                                        _world.GetPool<TargetEntity>().Add(tile).Value = _world.GetEntityLong(levelID);
                                        _world.GetPool<DeleteEntityCommand>().Add(tile);
                                    },
                                    continuationOptions: TaskContinuationOptions.ExecuteSynchronously);

                        GameField().Cells[counter++] = new GameField.Cell
                        {
                            View = view,
                            CellPosition = new float2(x, z),
                            WorldPosition = position
                        };
                    }

                    await Task.Delay(TimeSpan.FromSeconds(
                        generationAspect.GrowthWaves.Get(algorithm).GrowthDuration /
                        (GameField().Size * GameField().Size)));
                }
            }

            await Task.WhenAll(generationAspect.GrowthWaves.Get(algorithm).GrowthTasks);

            GridUtils.GameFieldEvent<GameFieldGeneratedRequest>(_world, target: levelID);
            GridUtils.GameFieldEvent<GameFieldGeneratedRequest>(_world, target: algorithm);

            levelAspect.GameFieldGeneratedMarker.Add(levelID);
        }

        private async Task GrowTile(GameObject tile, GenerationAspect generationAspect, int algorithm, int levelID,
            TargetLevelAspect levelAspect)
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            ref GameField GameField() =>
                ref levelAspect.GameFields.Get(levelID);

            float3 initialScale = float3.zero;

            float3 targetScale = new float3(
                GameField().CellSize,
                1,
                GameField().CellSize);

            float elapsedTime = 0;

            while (elapsedTime < generationAspect.GrowthWaves.Get(algorithm).GrowthDuration)
            {
                elapsedTime += Time.deltaTime;

                tile.transform.localScale =
                    math.lerp(initialScale, targetScale,
                        elapsedTime / generationAspect.GrowthWaves.Get(algorithm).GrowthDuration);

                await Task.Yield();
            }

            tile.transform.localScale = targetScale;
        }

        private async void Destruct(DestructionAspect destructionAspect, TargetLevelAspect levelAspect, int algorithm)
        {
            if (!destructionAspect.Targets.Read(algorithm).Value.TryGetID(out int levelID) ||
                !levelAspect.IsMatches(levelID))
                return;

            levelAspect.GameFieldGeneratedMarker.Del(levelID);

            destructionAspect.GrowthWaves.Get(algorithm).ShrinkTasks ??= new Task[GameField().CellsCount];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            ref GameField GameField() =>
                ref levelAspect.GameFields.Get(levelID);

            GameField.Cell[] cells = GameField().Cells;

            for (int index = 0; index < cells.Length; index++)
            {
                destructionAspect.GrowthWaves.Get(algorithm).ShrinkTasks[index] =
                    ShrinkTile(cells[index].View, destructionAspect, algorithm);

                await Task.Delay(TimeSpan.FromSeconds(
                    destructionAspect.GrowthWaves.Get(algorithm).GrowthDuration /
                    (GameField().Size * GameField().Size)));
            }

            await Task.WhenAll(destructionAspect.GrowthWaves.Get(algorithm).ShrinkTasks);

            GridUtils.GameFieldEvent<GameFieldDestructedRequest>(_world, target: levelID);
            GridUtils.GameFieldEvent<GameFieldDestructedRequest>(_world, target: algorithm);
            
            levelAspect.GameFieldDestructedMarker.Add(levelID);
        }

        private async Task ShrinkTile(GameObject tile, DestructionAspect generationAspect, int algorithm)
        {
            float3 initialScale = tile.transform.localScale;

            float3 targetScale = float3.zero;

            float elapsedTime = 0;

            while (elapsedTime < generationAspect.GrowthWaves.Get(algorithm).GrowthDuration)
            {
                elapsedTime += Time.deltaTime;

                tile.transform.localScale =
                    math.lerp(initialScale, targetScale,
                        elapsedTime / generationAspect.GrowthWaves.Get(algorithm).GrowthDuration);

                await Task.Yield();
            }

            Object.Destroy(tile);
        }
    }
}