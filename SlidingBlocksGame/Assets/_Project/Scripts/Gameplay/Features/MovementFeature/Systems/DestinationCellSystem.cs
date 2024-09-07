using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    public class DestinationCellSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class AnimalAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalTag))]
            [Inc] public readonly EcsPool<MovementDirection> Directions;

            [Inc] public readonly EcsPool<ActiveGameField> ActiveGameFields;

            [Inc] public readonly EcsPool<CellPosition> CellPositions;

            [Opt] public readonly EcsPool<CellDestination> CellDestination;
            [Opt] public readonly EcsPool<WorldDestination> WorldDestination;
        }

        private class ObstacleAnimalAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalTag))]
            [Inc] public readonly EcsPool<MovementDirection> Directions;

            [Inc] public readonly EcsPool<ActiveGameField> ActiveGameFields;
            [Inc] public readonly EcsPool<CellDestination> CellDestinations;
        }

        private class ClickAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ClickSideMarker))]
            [Inc] public readonly EcsPool<WorldPosition> WorldPositions;

            [Inc] public readonly EcsPool<ActiveGameField> ActiveGameFields;
        }

        private class GameFieldAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CanClickGameFieldMarker))]
            [Inc] public readonly EcsPool<GameField> GameFields;
        }

        private class GameAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameTag))]
            [Inc] public readonly EcsPool<MovementStrategyCfg> MovementStrategyConfigs;
        }

        private class SideAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(SideTag))]
            [Inc] public readonly EcsPool<SideAnimals> SideAnimals;

            [Inc] public readonly EcsPool<MovementDirection> MovementDirections;
            [Exc] public readonly EcsTagPool<SideProcessedMarker> SideProcessed;
        }

        public void Run()
        {
            foreach (int click in _world.Where(out ClickAspect clickAspect))
            {
                GameFieldAspect gameFieldAspect = _world.GetAspect<GameFieldAspect>();

                if (!clickAspect.ActiveGameFields.Read(click).Value.TryGetID(out int gameFieldID) &&
                    gameFieldAspect.IsMatches(gameFieldID))
                    continue;

                ref readonly WorldPosition clickPosition = ref clickAspect.WorldPositions.Read(click);

                ref readonly var gameField = ref gameFieldAspect.GameFields.Read(gameFieldID);

                float2 worldToGridPosition = GridUtils.GetCellPosition(clickPosition.Value, in gameField);

                int2 invertedSide = GridUtils.GetInvertedSide(
                    position: worldToGridPosition,
                    gameField: in gameField);

                EcsSpan where = _world.Where(out AnimalAspect animalAspect);

                List<int2> obstacles =
                    new List<int2>(where.Count);

                entlong side = default;

                SideAspect sideAspect;

                foreach (entlong t in _world.Where(out sideAspect).Longs)
                {
                    if (math.all(sideAspect.MovementDirections.Read(t.ID).Value == invertedSide))
                    {
                        side = t;

                        break;
                    }
                }

                if (!side.TryGetID(out int sideID) || sideAspect.SideAnimals.Get(sideID).Value.Count == 0)
                    continue;

                sideAspect.SideProcessed.Add(sideID);

                ref SideAnimals sideAnimals = ref sideAspect.SideAnimals.Get(sideID);

                foreach (int animal in sideAnimals.Value)
                {
                    ref readonly var cellPosition = ref animalAspect.CellPositions.Read(animal);

                    float2 end = default;

                    if (cellPosition.Value.x < gameField.EdgeSize)
                        end = new float2(3, cellPosition.Value.y);
                    else if (cellPosition.Value.y < gameField.EdgeSize)
                        end = new float2(cellPosition.Value.x, 3);
                    else if (cellPosition.Value.x >= gameField.EdgeSize + gameField.CenterSize)
                        end = new float2(gameField.EdgeSize, cellPosition.Value.y);
                    else if (cellPosition.Value.y >= gameField.EdgeSize + gameField.CenterSize)
                        end = new float2(cellPosition.Value.x, gameField.EdgeSize);

                    float2 progress;

                    if (invertedSide.x == 1 || invertedSide.y == 1)
                        progress = gameField.EdgeSize * invertedSide - cellPosition.Value * invertedSide +
                                   cellPosition.Value;
                    else
                        progress = (gameField.EdgeSize + gameField.CenterSize - 1) * math.abs(invertedSide) -
                                   cellPosition.Value * math.abs(invertedSide) +
                                   cellPosition.Value;
                    int step = 0;

                    do
                    {
                        progress += invertedSide * step;

                        Debug.Log(progress);

                        foreach (int obstacle in _world.Where(out ObstacleAnimalAspect obstacleAnimalAspect))
                        {
                            ref readonly CellDestination obstacleCellPosition =
                                ref obstacleAnimalAspect.CellDestinations.Read(obstacle);

                            if (math.all(obstacleCellPosition.Value == progress))
                            {
                                obstacles.Add(obstacleCellPosition.Value);
                            }
                        }

                        step++;
                    } while (math.any(progress != end));
                }


                if (obstacles.Count == 0)
                {
                    Debug.Log("no obstacles" + "============");

                    int2 max = sideAnimals.Value
                        .Select(x => _world.GetPool<CellPosition>().Read(x).Value * invertedSide)
                        .Aggregate((x, y) => math.max(x, y));

                    Debug.Log(max);

                    max = math.abs(max);

                    int2 center = GridUtils.GetCenter(in gameField);

                    if (invertedSide.x == 1)
                    {
                        center = center.yx;
                    }
                    else if (invertedSide.x == -1)
                    {
                        center = center.xx;
                    }
                    else if (invertedSide.y == 1)
                    {
                        center = center.xy;
                    }
                    else if (invertedSide.y == -1)
                    {
                        center = center.xx;
                    }

                    int2 distance = center * math.abs(invertedSide) - max;

                    Debug.Log(distance);

                    foreach (int animal in sideAnimals.Value)
                    {
                        ref CellDestination cellDestination = ref animalAspect.CellDestination.Add(animal);

                        cellDestination.Value = animalAspect.CellPositions.Read(animal).Value + distance;

                        float3 destination = GridUtils.GetWorldPosition(cellDestination.Value, in gameField);

                        animalAspect.WorldDestination.Add(animal).Value =
                            new float3(destination.x,
                                _world.GetPool<GameObjectConnect>().Read(animal).Connect.transform.position.y,
                                destination.z);

                        _world.GetPool<CanMoveMarker>().Add(animal);
                    }
                }
                else
                {
                    int2 minObstacle = obstacles.Select(x => x * invertedSide).Aggregate((x, y) => math.min(x, y));

                    int2 max = sideAnimals.Value
                        .Select(x => _world.GetPool<CellPosition>().Read(x).Value * invertedSide)
                        .Aggregate((x, y) => math.max(x, y));

                    max = math.abs(max);
                    minObstacle = math.abs(minObstacle);

                    Debug.Log(minObstacle + "============");

                    int2 distance = minObstacle * math.abs(invertedSide) - max * math.abs(invertedSide) -
                                    invertedSide;

                    foreach (int animal in sideAnimals.Value)
                    {
                        ref var cellDestination = ref animalAspect.CellDestination.Add(animal);

                        Debug.Log(distance);
                        cellDestination.Value =
                            animalAspect.CellPositions.Read(animal).Value + distance;

                        float3 destination = GridUtils.GetWorldPosition(cellDestination.Value, in gameField);

                        animalAspect.WorldDestination.Add(animal).Value =
                            new float3(destination.x,
                                _world.GetPool<GameObjectConnect>().Read(animal).Connect.transform.position.y,
                                destination.z);

                        _world.GetPool<CanMoveMarker>().Add(animal);
                    }
                }

                foreach (int game in _world.Where(out GameAspect gameAspect))
                {
                    int movementStrategy = _world.NewEntity(gameAspect.MovementStrategyConfigs.Read(game).Value);
                    _world.GetPool<ApplyStrategyRequest>().Add(movementStrategy);
                }
            }
        }
    }
}