using System;
using System.Linq;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Extensions;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Utils;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    public class CalculateDestinationSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class AnimalAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(MovableMarker))]
            [ExcImplicit(typeof(MovingMarker))]
            [Inc] public readonly EcsPool<MovementDirection> Directions;

            [Inc] public readonly EcsPool<ActiveGameField> ActiveGameFields;

            [Inc] public readonly EcsPool<CellPosition> CellPositions;

            [Opt] public readonly EcsPool<CellDestination> CellDestination;
            [Opt] public readonly EcsPool<WorldDestination> WorldDestination;
        }

        private class ClickAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(SideClickedEvent))]
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
                GameFieldAspect gameFieldAspect = _world.GetAspect<GameFieldAspect>();

                if (!clickAspect.ActiveGameFields.Read(click).Value.TryGetID(out int gameFieldID) ||
                    !gameFieldAspect.IsMatches(gameFieldID))
                    continue;

                ref readonly WorldPosition clickPosition = ref clickAspect.WorldPositions.Read(click);

                ref GameField gameField = ref gameFieldAspect.GameFields.Get(gameFieldID);

                float2 worldToGridPosition = GridUtils.GetCellPosition(clickPosition.Value, gameField.ToGrid());

                int2 invertedSide = GridUtils.GetInvertedSide(
                    position: worldToGridPosition,
                    edgeSize: gameField.EdgeSize,
                    centerSize: gameField.CenterSize);

                (int2 distance, bool success) minDistance = new ValueTuple<int2, bool>(int.MaxValue, false);

                EcsGroup animals = EcsGroup.New(_world);

                AnimalAspect animalAspect;

                foreach (int animal in _world.Where(out animalAspect))
                {
                    ref readonly MovementDirection movementDirection = ref animalAspect.Directions.Read(animal);

                    if (!math.all(movementDirection.Value == invertedSide))
                        continue;

                    ref readonly CellPosition cellPosition = ref animalAspect.CellPositions.Read(animal);

                    minDistance = FindMinDistance(
                        invertedSide: invertedSide,
                        gameField: in gameField,
                        cellPosition: in cellPosition,
                        minDistance:  minDistance.distance);

                    animals.Add(animal);
                }

                if (animals.Count == 0)
                    continue;

                Debug.Log(minDistance.distance);
                
                if (!minDistance.success)
                {
                    int2 max = animals
                        .Select(x => _world.GetPool<CellPosition>().Read(x).Value * invertedSide)
                        .Aggregate(math.max);

                    int2 center;

                    if (math.any(invertedSide < 0))
                        center = gameField.EdgeSize;
                    else
                        center = gameField.EdgeSize + gameField.CenterSize - 1;

                    int2 distance = center * math.abs(invertedSide) - math.abs(max);

                    foreach (int animal in animals)
                    {
                        ref CellDestination cellDestination = ref animalAspect.CellDestination.Add(animal);

                        cellDestination.Value = animalAspect.CellPositions.Read(animal).Value + distance;

                        float3 destination = GridUtils.GetWorldPosition(cellDestination.Value, gameField.ToGrid());

                        animalAspect.WorldDestination.Add(animal).Value =
                            new float3(destination.x,
                                _world.GetPool<GameObjectConnect>().Read(animal).Connect.transform.position.y,
                                destination.z);

                        GridUtils.SetCell(
                            position: cellDestination.Value,
                            edgeSize: gameField.EdgeSize,
                            grid: ref gameField.Center);
                    }
                }
                else
                {
                    minDistance.distance -= invertedSide;

                    foreach (int animal in animals)
                    {
                        ref CellDestination cellDestination = ref animalAspect.CellDestination.Add(animal);

                        cellDestination.Value =
                            animalAspect.CellPositions.Read(animal).Value + minDistance.distance;

                        float3 destination = GridUtils.GetWorldPosition(cellDestination.Value, gameField.ToGrid());

                        animalAspect.WorldDestination.Add(animal).Value =
                            new float3(
                                destination.x,
                                _world.GetPool<GameObjectConnect>().Read(animal).Connect.transform.position.y,
                                destination.z);

                        Debug.Log(cellDestination.Value);
                        Debug.Log(minDistance.distance);

                        GridUtils.SetCell(
                            position: cellDestination.Value,
                            edgeSize: gameField.EdgeSize,
                            grid: ref gameField.Center);
                    }
                }

                foreach (int game in _world.Where(out GameAspect gameAspect))
                {
                    ref readonly MovementStrategyCfg movementStrategyCfg =
                        ref gameAspect.MovementStrategyConfigs.Read(game);
                    int movementStrategy = _world.NewEntity(movementStrategyCfg.Value);
                    _world.GetPool<ApplyMovementStrategyRequest>().Add(movementStrategy);
                    _world.GetPool<TargetEntities>().Add(movementStrategy).Value = animals;
                }
            }
        }

        private static (int2 distance, bool success) FindMinDistance(
            int2 invertedSide,
            in GameField gameField,
            in CellPosition cellPosition,
            int2 minDistance)
        {
            (int2 obstacle, bool success) nearest =
                GridUtils.GetNearestCentralObstacle(
                    cellPosition.Value,
                    invertedSide,
                    gameField.EdgeSize,
                    gameField.Center);

            int2 convertedObstacle = nearest.obstacle + gameField.EdgeSize;

            int2 distance = convertedObstacle - cellPosition.Value;

            if (math.any(math.abs(minDistance) > math.abs(distance)) && nearest.success)
                minDistance = distance;

            return (minDistance, nearest.success);
        }
    }
}