using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
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
            [Inc] public readonly EcsPool<GameField> GameFields;
        }

        private class GameAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameTag))]
            [Inc] public readonly EcsPool<MovementStrategyCfg> MovementStrategyConfigs;
        }

        public void Run()
        {
            foreach (int click in _world.Where(out ClickAspect clickAspect))
            {
                if (!clickAspect.ActiveGameFields.Read(click).Value.TryGetID(out int gameFieldID))
                    continue;

                GameFieldAspect gameFieldAspect = _world.GetAspect<GameFieldAspect>();

                ref readonly WorldPosition clickPosition = ref clickAspect.WorldPositions.Read(click);

                ref readonly var gameField = ref gameFieldAspect.GameFields.Read(gameFieldID);

                float2 worldToGridPosition = GridUtils.GetCellPosition(clickPosition.Value, in gameField);

                int2 invertedSide = GridUtils.GetInvertedSide(
                    position: worldToGridPosition,
                    gameField: in gameField);

                EcsSpan where = _world.Where(out AnimalAspect animalAspect);

                EcsGroup filteredAnimals = EcsGroup.New(_world);

                List<(entlong animal, int2 obstacle)> obstacles =
                    new List<(entlong animal, int2 obstacle)>(where.Count);

                foreach (int animal in where)
                {
                    if (!math.all(animalAspect.Directions.Read(animal).Value == invertedSide))
                        continue;

                    filteredAnimals.Add(animal);

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
                                obstacles.Add((animal.ToEntityLong(_world),
                                    obstacleCellPosition.Value));
                            }
                        }

                        step++;
                    } while (math.any(progress != end));
                }

                if (obstacles.Count == 0)
                {
                    Debug.Log("no obstacles" + "============");

                    foreach (int animal in filteredAnimals)
                    {
                        ref CellDestination cellDestination = ref animalAspect.CellDestination.Add(animal);

                        cellDestination.Value = gameField.EdgeSize * invertedSide +
                                                animalAspect.CellPositions.Read(animal).Value;

                        animalAspect.WorldDestination.Add(animal).Value =
                            GridUtils.GetWorldPosition(cellDestination.Value, in gameField);

                        _world.GetPool<CanMoveMarker>().Add(animal);
                    }
                }
                else
                {
                    (entlong animal, int2 obstacle) min = obstacles.ToArray().Min();

                    Debug.Log(min + "============");
                    int2 distance =
                        (min.obstacle * math.abs(invertedSide) - animalAspect.CellPositions.Read(min.animal.ID).Value -
                         invertedSide) * invertedSide;

                    foreach (int animal in filteredAnimals)
                    {
                        ref var cellDestination = ref animalAspect.CellDestination.Add(animal);

                        Debug.Log(distance);
                        cellDestination.Value =
                            animalAspect.CellPositions.Read(animal).Value + distance * invertedSide;

                        animalAspect.WorldDestination.Add(animal).Value =
                            GridUtils.GetWorldPosition(cellDestination.Value, in gameField);

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