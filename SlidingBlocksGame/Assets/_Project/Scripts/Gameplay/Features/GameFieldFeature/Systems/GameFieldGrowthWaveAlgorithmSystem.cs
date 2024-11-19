using System.Collections.Generic;
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
    public class GameFieldGrowthWaveAlgorithmSystem : IEcsInit, IEcsRun
    {
        [EcsInject] private EcsWorld _world;
        private DragonCoroutineRunner _dragonCoroutineRunner;

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
                _dragonCoroutineRunner.StartCoroutine(
                    Generate(
                        generationAspect: aspect,
                        gameFieldAspect: _world.GetAspect<GameFieldAspect>(),
                        algorithm: entity));
            }

            foreach (int entity in _world.Where(out DestructionAspect aspect))
            {
                _dragonCoroutineRunner.StartCoroutine(
                    Destruct(
                        destructionAspect: aspect,
                        gameFieldAspect: _world.GetAspect<GameFieldAspect>(),
                        algorithm: entity));
            }
        }

        private IEnumerator<CustomYieldInstruction> Generate(GenerationAspect generationAspect,
            GameFieldAspect gameFieldAspect, int algorithm)
        {
            if (!generationAspect.Targets.Read(algorithm).Value.TryGetID(out int gameFieldID) ||
                !gameFieldAspect.IsMatches(gameFieldID))
                yield break;

            int counter = 0;

            gameFieldAspect.GameFieldDestructedMarker.TryDel(gameFieldID);

            generationAspect.GrowthWaves.Get(algorithm).GrowthCoroutines ??=
                new DragonCoroutine[GameField().CellsCount];

            generationAspect.GrowthWaves.Get(algorithm).SpeedFactor =
                generationAspect.GrowthWaves.Get(algorithm).BaseSpeedFactor *
                ((float)GameField().BaseSize / GameField().Size);

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

                        GameObject view = Object.Instantiate(
                            original: GameField().CellPrefab,
                            position: position,
                            rotation: Quaternion.identity);

                        view.transform.localScale = float3.zero;

                        _dragonCoroutineRunner.StartCoroutine(
                            GrowTileCoroutine(generationAspect, gameFieldAspect,
                                algorithm, counter, view, gameFieldID));

                        GameField().Cells[counter++] = new GameField.Cell
                        {
                            View = view,
                            WorldPosition = position
                        };
                    }

                    yield return new DragonAPI.YieldInstructions.DragonAPI.WaitForSeconds(
                        generationAspect.GrowthWaves.Get(algorithm).SpeedFactor /
                        (GameField().Size * GameField().Size));
                }
            }

            yield return new DragonAPI.YieldInstructions.DragonAPI.WhenAll(
                generationAspect.GrowthWaves.Get(algorithm).GrowthCoroutines);

            gameFieldAspect.GameFieldGeneratedByAlgorithm.TryAddOrGet(gameFieldID).Value =
                algorithm.ToEntityLong(_world);

            gameFieldAspect.GameFieldGeneratedMarker.Add(gameFieldID);
            gameFieldAspect.GameFieldGeneratedEvent.Add(gameFieldID);

            yield break;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            ref GameField GameField() =>
                ref gameFieldAspect.GameFields.Get(gameFieldID);
        }

        private IEnumerator<CustomYieldInstruction> GrowTileCoroutine(
            GenerationAspect generationAspect,
            GameFieldAspect gameFieldAspect,
            int algorithm,
            int counter,
            GameObject view,
            int gameFieldID)
        {
            yield return new DragonAPI.YieldInstructions.DragonAPI.WaitForCoroutine(
                generationAspect.GrowthWaves.Get(algorithm).GrowthCoroutines[counter] =
                    _dragonCoroutineRunner.StartCoroutine(
                        GrowTile(view, generationAspect, algorithm, gameFieldID, gameFieldAspect)));

            int tile = _world.NewEntity();

            _world.GetPool<TileGeneratedEvent>().Add(tile);
            _world.GetPool<TargetEntity>().Add(tile).Value =
                _world.GetEntityLong(gameFieldID);
        }

        private static IEnumerator<CustomYieldInstruction> GrowTile(
            GameObject tile,
            GenerationAspect generationAspect,
            int algorithm,
            int gameFieldID,
            GameFieldAspect gameFieldAspect)
        {
            float3 initialScale = float3.zero;

            float3 targetScale = new float3(GameField().CellSize, GameField().CellScaleY, GameField().CellSize);

            float elapsedTime = 0;

            while (elapsedTime < generationAspect.GrowthWaves.Get(algorithm).SpeedFactor)
            {
                elapsedTime += Time.deltaTime;

                tile.transform.localScale =
                    math.lerp(initialScale, targetScale,
                        elapsedTime / generationAspect.GrowthWaves.Get(algorithm).SpeedFactor);

                yield return null;
            }

            tile.transform.localScale = targetScale;

            yield break;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            ref GameField GameField() =>
                ref gameFieldAspect.GameFields.Get(gameFieldID);
        }

        private IEnumerator<CustomYieldInstruction> Destruct(DestructionAspect destructionAspect,
            GameFieldAspect gameFieldAspect, int algorithm)
        {
            if (!destructionAspect.Targets.Read(algorithm).Value.TryGetID(out int gameFieldID) ||
                !gameFieldAspect.IsMatches(gameFieldID))
                yield break;

            gameFieldAspect.GameFieldGeneratedMarker.Del(gameFieldID);

            destructionAspect.GrowthWaves.Get(algorithm).ShrinkCoroutines ??=
                new DragonCoroutine[GameField().CellsCount];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            ref GameField GameField() =>
                ref gameFieldAspect.GameFields.Get(gameFieldID);

            GameField.Cell[] cells = GameField().Cells;

            for (int index = 0; index < cells.Length; index++)
            {
                destructionAspect.GrowthWaves.Get(algorithm).ShrinkCoroutines[index] =
                    _dragonCoroutineRunner.StartCoroutine(ShrinkTile(cells[index].View, destructionAspect, algorithm));

                yield return new DragonAPI.YieldInstructions.DragonAPI.WaitForSeconds(
                    destructionAspect.GrowthWaves.Get(algorithm).SpeedFactor /
                    (GameField().Size * GameField().Size));
            }

            yield return new DragonAPI.YieldInstructions.DragonAPI.WhenAll(destructionAspect.GrowthWaves.Get(algorithm)
                .ShrinkCoroutines);

            gameFieldAspect.GameFieldDestructedMarker.Add(gameFieldID);
            gameFieldAspect.GameFieldDestructedEvent.Add(gameFieldID);

            gameFieldAspect.GameFieldGeneratedByAlgorithm.Del(gameFieldID);
        }

        private static IEnumerator<CustomYieldInstruction> ShrinkTile(GameObject tile,
            DestructionAspect generationAspect, int algorithm)
        {
            float3 initialScale = tile.transform.localScale;

            float3 targetScale = float3.zero;

            float elapsedTime = 0;

            while (elapsedTime < generationAspect.GrowthWaves.Get(algorithm).SpeedFactor)
            {
                elapsedTime += Time.deltaTime;

                tile.transform.localScale =
                    math.lerp(initialScale, targetScale,
                        elapsedTime / generationAspect.GrowthWaves.Get(algorithm).SpeedFactor);

                yield return null;
            }

            Object.Destroy(tile);
        }
    }
}