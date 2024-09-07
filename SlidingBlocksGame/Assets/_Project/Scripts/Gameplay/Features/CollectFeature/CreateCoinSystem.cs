using System;
using _Project.Scripts.Gameplay.Features.CollectFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;
using PrimeTween;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

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

        Random random = new Random((uint)Environment.TickCount);

        public void Run()
        {
            foreach (int entity in _world.Where(out LevelAspect levelAspect))
            {
                foreach (var game in _world.Where(out GameAspect gameAspect))
                {
                    ref readonly GameField gameField = ref levelAspect.GameFields.Read(entity);

                    int2 center = GridUtils.GetCenter(in gameField);

                    int randomX = random.NextInt(center.x, center.y);
                    int randomY = random.NextInt(center.x, center.y);

                    int2 cellPosition = new int2(randomX, randomY);
                    
                    EcsEntityConnect connect = UnityEngine.Object.Instantiate(gameAspect.CoinPrefabs.Read(game).Prefab,
                        GridUtils.GetWorldPosition(cellPosition, in gameField) +
                        new float3(0, gameField.CellTop + gameField.UnitCellTopOffset, 0), Quaternion.identity);

                    connect.transform.localScale = new Vector3(gameField.CellSize + 0.1f, gameField.CellSize + 0.1f,
                        gameField.CellSize + 0.1f) / 2f;

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