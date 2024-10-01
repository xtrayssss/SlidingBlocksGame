using _Project.Scripts.Gameplay.Features.CollectFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;
using PrimeTween;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CollectFeature
{
    public class CreateCoinSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

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
                foreach (var game in _world.Where(out GameAspect gameAspect))
                {
                    ref readonly GameField gameField = ref levelAspect.GameFields.Read(entity);

                    int2 center = GridUtils.GetCenter(in gameField) + new int2(0, 1);

                    int randomX = Random.Range(center.x, center.y);
                    int randomY = Random.Range(center.x, center.y);

                    int2 cellPosition = new int2(randomX, randomY);

                    float3 cellUpperOffset = new float3(
                        0,
                        gameField.CellScaleY + gameField.UnitCellTopOffset,
                        0);

                    float3 worldPosition = GridUtils.GetWorldPosition(cellPosition, in gameField) + cellUpperOffset;

                    EcsEntityConnect connect =
                        Object.Instantiate(
                            original: gameAspect.CoinPrefabs.Read(game).Prefab,
                            position: worldPosition,
                            rotation: Quaternion.identity);

                    Tween.Scale(
                            target: connect.transform,
                            startValue: Vector3.zero,
                            endValue: new Vector3(
                                gameField.CellSize + 0.1f,
                                gameField.CellSize + 0.1f,
                                gameField.CellSize + 0.1f) / 2f,
                            duration: 0.4f,
                            ease: Ease.InOutSine)
                        .OnComplete(
                            target: connect,
                            static target =>
                            {
                                if (!target.Entity.TryGetID(out int _))
                                    return;

                                EcsWorld world = target.Entity.World;

                                int catcher = world.NewEntity();

                                CoinCatcherAspect.CoinSpawnedCatcher catcherAspect =
                                    world.GetAspect<CoinCatcherAspect.CoinSpawnedCatcher>();

                                catcherAspect.CommonCatcherAspect.TargetEntities.Add(catcher).Value = target.Entity;
                                catcherAspect.CatchCoinSpawnedRequest.Add(catcher);
                            });

                    entlong coin = _world.NewEntityLong();

                    var coinAspect = _world.GetAspect<CoinAspect>();

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
    }
}