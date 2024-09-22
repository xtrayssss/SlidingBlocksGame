using System;
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

                if (!clickAspect.ActiveGameFields.Read(click).Value.TryGetID(out int gameFieldID) ||
                    !gameFieldAspect.IsMatches(gameFieldID))
                    continue;

                ref readonly WorldPosition clickPosition = ref clickAspect.WorldPositions.Read(click);

                ref readonly var gameField = ref gameFieldAspect.GameFields.Read(gameFieldID);

                float2 worldToGridPosition = GridUtils.GetCellPosition(clickPosition.Value, in gameField);

                int2 invertedSide = GridUtils.GetInvertedSide(
                    position: worldToGridPosition,
                    gameField: in gameField);

                _world.Where(out AnimalAspect animalAspect);
                
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

                bool hasNearest = false;
                int2 minDistance = int.MaxValue;

                foreach (int animal in sideAnimals.Value)
                {
                    ref readonly CellPosition cellPosition = ref animalAspect.CellPositions.Read(animal);

                    (int2 obstacle, bool success) nearest =
                        GridUtils.GetNearestCentralObstacle(cellPosition.Value, invertedSide, in gameField);

                    int2 convertedObstacle = nearest.obstacle + gameField.EdgeSize;

                    int2 distance = convertedObstacle - cellPosition.Value;
                    
                    if (math.any(math.abs(minDistance) > math.abs(distance)) && nearest.success)
                    {
                        minDistance = distance;
                        hasNearest = true;
                    }

                    Debug.Log(convertedObstacle);
                }

                if (!hasNearest)
                {
                    int2 max = sideAnimals.Value
                        .Select(x => _world.GetPool<CellPosition>().Read(x).Value * invertedSide)
                        .Aggregate((x, y) => math.max(x, y));

                    int2 center;

                    if (math.any(invertedSide < 0))
                        center = gameField.EdgeSize;
                    else
                        center = gameField.EdgeSize + gameField.CenterSize - 1;

                    int2 distance = center * math.abs(invertedSide) - math.abs(max);

                    foreach (int animal in sideAnimals.Value)
                    {
                        ref CellDestination cellDestination = ref animalAspect.CellDestination.Add(animal);

                        cellDestination.Value = animalAspect.CellPositions.Read(animal).Value + distance;

                        float3 destination = GridUtils.GetWorldPosition(cellDestination.Value, in gameField);

                        animalAspect.WorldDestination.Add(animal).Value =
                            new float3(destination.x,
                                _world.GetPool<GameObjectConnect>().Read(animal).Connect.transform.position.y,
                                destination.z);
                                                
                        GridUtils.SetCell(
                            position: cellDestination.Value,
                            gameField: ref gameFieldAspect.GameFields.Get(gameFieldID));

                        _world.GetPool<CanMoveMarker>().Add(animal);
                    }
                }
                else
                {
                    minDistance -= invertedSide;

                    foreach (int animal in sideAnimals.Value)
                    {
                        ref CellDestination cellDestination = ref animalAspect.CellDestination.Add(animal);

                        cellDestination.Value =
                            animalAspect.CellPositions.Read(animal).Value + minDistance;

                        float3 destination = GridUtils.GetWorldPosition(cellDestination.Value, in gameField);

                        animalAspect.WorldDestination.Add(animal).Value =
                            new float3(
                                destination.x,
                                _world.GetPool<GameObjectConnect>().Read(animal).Connect.transform.position.y,
                                destination.z);
                        
                        GridUtils.SetCell(
                            position: cellDestination.Value,
                            gameField: ref gameFieldAspect.GameFields.Get(gameFieldID));

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