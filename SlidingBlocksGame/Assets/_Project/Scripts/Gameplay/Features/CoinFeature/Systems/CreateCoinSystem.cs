using _Project.Scripts.Gameplay.Features.CoinFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Extensions;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Utils;
using DCFApixels.DragonECS;
using PrimeTween;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Gameplay.Features.CoinFeature.Systems
{
    public class CreateCoinSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private static readonly int2 CENTER_RANGE_CORRECTION = new int2(0, 1);
        private const float COIN_SCALE_FACTOR = 0.5f;
        private const float COIN_SIZE_OFFSET = 0.1f;

        private class LevelAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CreateCoinRequest))]
            [Inc] public readonly EcsPool<GameField> GameFields;
        }

        private class GameAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<CoinPrefab> CoinPrefabs;
        }

        private class CoinAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsPool<CellPosition> CellPosition;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out LevelAspect levelAspect))
            {
                foreach (int game in _world.Where(out GameAspect gameAspect))
                {
                    ref readonly GameField gameField = ref levelAspect.GameFields.Read(entity);

                    int2 center = GridUtils.GetCenter(
                        edgeSize: gameField.EdgeSize,
                        centerSize: gameField.CenterSize) + CENTER_RANGE_CORRECTION;

                    int randomX = Random.Range(center.x, center.y);
                    int randomY = Random.Range(center.x, center.y);

                    int2 cellPosition = new int2(randomX, randomY);

                    float3 cellUpperOffset = new float3(
                        0,
                        gameField.CellScaleY + gameField.UnitCellTopOffset,
                        0);

                    float3 worldPosition = GetWorldPosition(cellPosition, gameField, cellUpperOffset);

                    EcsEntityConnect connect =
                        Object.Instantiate(
                            original: gameAspect.CoinPrefabs.Read(game).Prefab,
                            position: worldPosition,
                            rotation: Quaternion.identity);

                    float size = gameField.CellSize + COIN_SIZE_OFFSET;

                    Tween.Scale(
                            target: connect.transform,
                            startValue: Vector3.zero,
                            endValue: new Vector3(size, size, size) * COIN_SCALE_FACTOR,
                            duration: 0.4f,
                            ease: Ease.InOutSine)
                        .OnComplete(
                            target: connect,
                            static target =>
                            {
                                if (!target.Entity.TryGetID(out int _))
                                    return;

                                EcsWorld world = target.World;

                                int catcher = world.NewEntity();

                                CoinCatcherAspect.CoinSpawnedCatcher catcherAspect =
                                    world.GetAspect<CoinCatcherAspect.CoinSpawnedCatcher>();

                                catcherAspect.CommonCatcherAspect.TargetEntities.Add(catcher).Value = target.Entity;
                                catcherAspect.CatchCoinSpawnedRequest.Add(catcher);
                            });

                    entlong coin = _world.NewEntityLong();

                    CoinAspect coinAspect = _world.GetAspect<CoinAspect>();

                    coinAspect.CellPosition.Add(coin.ID).Value = cellPosition;

                    connect.Connect(coin, applyTemplates: true);

                    Tween.LocalEulerAngles(
                        target: connect.transform,
                        startValue: Vector3.zero,
                        endValue: new Vector3(0, 360, 0),
                        duration: 4f,
                        ease: Ease.Linear,
                        cycles: -1,
                        cycleMode: CycleMode.Incremental);
                }
            }
        }

        private static float3 GetWorldPosition(int2 cellPosition, GameField gameField, float3 cellUpperOffset)
        {
            return GridUtils.GetWorldPosition(
                coordinates: cellPosition, 
                gameField.ToGrid()) + cellUpperOffset;
        }
    }
}