using System;
using System.Linq;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    public class CalculateDestinationCellSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class MaxDistanceAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CalculateDestinationCellRequest))]
            [IncImplicit(typeof(CalculationMaxDistanceRequest))]
            [Inc]
            public readonly EcsPool<CellPosition> CellPositions;

            [Inc] public readonly EcsPool<AssignedGroup> AssignedGroups;

            [Inc] public readonly EcsPool<MovementDirection> Directions;
            [Inc] public readonly EcsPool<ActiveGameField> AspectGameFields;

            [Exc] public readonly EcsPool<WorldDestination> WorldDestination;
            [Exc] public readonly EcsPool<CellDestination> CellDestination;
        }

        private class ObstacleDistanceAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CalculateDestinationCellRequest))]
            [IncImplicit(typeof(CalculateObstacleDistanceRequest))]
            [Inc]
            public readonly EcsPool<CellPosition> CellPositions;

            [Inc] public readonly EcsPool<AssignedGroup> AssignedGroups;
            [Inc] public readonly EcsPool<ActiveGameField> AspectGameFields;

            [Exc] public readonly EcsPool<WorldDestination> WorldDestination;
            [Exc] public readonly EcsPool<CellDestination> CellDestination;
        }

        private class AssignedGroupAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<MovementDirection> Directions;
            [Inc] public readonly EcsPool<NearDistance> NearDistances;
            [Inc] public readonly EcsPool<Units> Units;
        }

        private class GameFieldAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameField> GameFields;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out MaxDistanceAspect aspect))
            {
                ActiveGameField activeGameField = aspect.AspectGameFields.Read(entity);
                GameFieldAspect gameFieldAspect = _world.GetAspect<GameFieldAspect>();

                if (activeGameField.Value.TryGetID(out int activeGameFieldID))
                {
                    Debug.Log("Max");

                    ref readonly GameField gameField = ref gameFieldAspect.GameFields.Read(activeGameFieldID);

                    AssignedGroupAspect assignedGroupAspect = _world.GetAspect<AssignedGroupAspect>();

                    if (aspect.AssignedGroups.Read(entity).Value.TryGetID(out int assignedGroupID))
                    {
                        ref readonly var groupDir = ref assignedGroupAspect.Directions.Read(assignedGroupID);

                        ref Units units = ref assignedGroupAspect.Units.Get(assignedGroupID);

                        if (math.all(groupDir.Value == new float2(1, 0)))
                        {
                            Debug.Log("123");

                            units.Filtered = units.Value.OrderByDescending(unit =>
                                _world.GetPool<CellPosition>().Read(unit).Value.x).Select(x =>
                                _world.GetEntityLong(x));
                        }
                        else if (math.all(groupDir.Value == new float2(-1, 0)))
                        {
                            units.Filtered = units.Value.OrderByDescending(unit =>
                            {
                                if (_world.GetEntityLong(unit).TryGetID(out int unitID))
                                {
                                    return -_world.GetPool<CellPosition>().Read(unitID).Value.x;
                                }

                                return 0;
                            }).Select(x => _world.GetEntityLong(x)).ToList();
                        }
                        else if (math.all(groupDir.Value == new float2(0, 1)))
                        {
                            units.Filtered = units.Value.OrderByDescending(unit =>
                            {
                                if (_world.GetEntityLong(unit).TryGetID(out int unitID))
                                {
                                    return _world.GetPool<CellPosition>().Read(unitID).Value.y;
                                }

                                return 0;
                            }).Select(x => _world.GetEntityLong(x)).ToList();
                        }
                        else if (math.all(groupDir.Value == new float2(0, -1)))
                        {
                            units.Filtered = units.Value.OrderByDescending(unit =>
                            {
                                if (_world.GetEntityLong(unit).TryGetID(out int unitID))
                                {
                                    return -_world.GetPool<CellPosition>().Read(unitID).Value.y;
                                }

                                return 0;
                            }).Select(x => _world.GetEntityLong(x)).ToList();
                        }

                        entlong first = units.Filtered.First();

                        Debug.Log(first);

                        if (units.Filtered.Count() > 1)
                        {
                            if (first.TryGetID(out int firstID))
                            {
                                ref readonly CellDestination firstDestination = ref First(aspect, firstID, gameField);

                                ref readonly CellPosition firstCellPosition = ref aspect.CellPositions.Read(firstID);

                                foreach (entlong unit in units.Filtered)
                                {
                                    if (unit.TryGetID(out int unitID))
                                    {
                                        if (unitID != firstID)
                                        {
                                            var unitDirection = aspect.Directions.Read(unitID).Value;
                                            ref readonly CellPosition unitCellPosition =
                                                ref aspect.CellPositions.Read(unitID);

                                            float2 distance = firstCellPosition.Value - unitCellPosition.Value;

                                            var result = unitDirection *
                                                         (distance * aspect.Directions.Read(unitID).Value);

                                            Debug.Log(distance);
                                            Debug.Log(result);
                                            Debug.Log(firstDestination.Value);
                                            Debug.Log(unitCellPosition.Value);
                                            Debug.Log(firstCellPosition.Value);

                                            var invertedDirection = new float2(aspect.Directions.Read(unitID).Value.y,
                                                aspect.Directions.Read(unitID).Value.x);

                                            aspect.CellDestination.TryAddOrGet(unitID).Value = firstDestination.Value -
                                                (firstCellPosition.Value - unitCellPosition.Value) * invertedDirection *
                                                invertedDirection;

                                            Debug.Log(aspect.CellDestination.TryAddOrGet(unitID).Value);

                                            aspect.CellDestination.TryAddOrGet(unitID).Value -= result;

                                            aspect.WorldDestination.TryAddOrGet(unitID).Value =
                                                CrossGrid.GetWorldPosition(
                                                    coordinates: aspect.CellDestination.Get(unitID).Value,
                                                    field: gameField
                                                );
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            ref CellDestination cellDestination = ref aspect.CellDestination.TryAddOrGet(entity);
                            ref readonly CellPosition cellPosition = ref aspect.CellPositions.Read(entity);

                            float distance;

                            if (cellPosition.Value.x < gameField.EdgeSize)
                            {
                                cellDestination.Value = new float2(3, cellPosition.Value.y);

                                distance = gameField.EdgeSize - 1 - cellPosition.Value.x;
                            }
                            else if (cellPosition.Value.x >= gameField.EdgeSize + gameField.CenterSize)
                            {
                                cellDestination.Value = new float2(2, cellPosition.Value.y);

                                distance = (gameField.EdgeSize + gameField.CenterSize - 1) - cellPosition.Value.x;
                            }
                            else if (cellPosition.Value.y < gameField.EdgeSize)
                            {
                                cellDestination.Value = new float2(cellPosition.Value.x, 3);

                                distance = gameField.EdgeSize - 1 - cellPosition.Value.y;
                            }
                            else if (cellPosition.Value.y >= gameField.EdgeSize + gameField.CenterSize)
                            {
                                cellDestination.Value = new float2(cellPosition.Value.x, 2);

                                distance = (gameField.EdgeSize + gameField.CenterSize - 1) - cellPosition.Value.y;
                            }
                            else if (cellPosition.Value.y >= gameField.EdgeSize + gameField.CenterSize)
                            {
                                cellDestination.Value = new float2(cellPosition.Value.x, 2);
                            }

                            //cellDestination.Value -= distance * aspect.Directions.Read(entity).Value;

                            aspect.WorldDestination.TryAddOrGet(entity).Value =
                                CrossGrid.GetWorldPosition(
                                    coordinates: cellDestination.Value,
                                    field: gameField
                                );
                        }
                    }
                }
            }

            foreach (int entity in _world.Where(out ObstacleDistanceAspect aspect))
            {
                ActiveGameField activeGameField = aspect.AspectGameFields.Read(entity);
                GameFieldAspect gameFieldAspect = _world.GetAspect<GameFieldAspect>();

                if (activeGameField.Value.TryGetID(out int activeGameFieldID))
                {
                    Debug.Log("Obstacle Distance");

                    ref readonly GameField gameField = ref gameFieldAspect.GameFields.Read(activeGameFieldID);
                    ref readonly CellPosition cellPosition = ref aspect.CellPositions.Read(entity);

                    ref CellDestination cellDestination = ref aspect.CellDestination.Add(entity);

                    AssignedGroupAspect assignedGroupAspect = _world.GetAspect<AssignedGroupAspect>();

                    if (!aspect.AssignedGroups.Read(entity).Value.TryGetID(out int assignedGroupID) ||
                        !assignedGroupAspect.IsMatches(assignedGroupID))
                        continue;

                    ref readonly NearDistance nearDistance =
                        ref assignedGroupAspect.NearDistances.Read(assignedGroupID);

                    cellDestination.Value = cellPosition.Value +
                                            (assignedGroupAspect.Directions.Read(assignedGroupID).Value *
                                             nearDistance.Value) - new float2(1, 1) * assignedGroupAspect.Directions
                                                .Read(assignedGroupID).Value;

                    Debug.Log(cellPosition.Value);
                    Debug.Log(nearDistance.Value);
                    Debug.Log(cellDestination.Value);
                    //
                    // cellDestination.Value +=
                    //     assignedGroupAspect.Directions.Read(assignedGroupID).Value * new float2(1, 1);
                    //
                    // Debug.Log(cellDestination.Value);


                    aspect.WorldDestination.Add(entity).Value =
                        CrossGrid.GetWorldPosition(
                            coordinates: cellDestination.Value,
                            field: gameField
                        );
                }
            }
        }

        private static ref readonly CellDestination First(MaxDistanceAspect aspect, int first, GameField gameField)
        {
            float distance = 0;

            ref readonly CellPosition cellPosition = ref aspect.CellPositions.Read(first);

            ref CellDestination cellDestination = ref aspect.CellDestination.TryAddOrGet(first);

            if (cellPosition.Value.x < gameField.EdgeSize)
            {
                cellDestination.Value = new float2(3, cellPosition.Value.y);

                distance = gameField.EdgeSize - 1 - cellPosition.Value.x;
            }
            else if (cellPosition.Value.x >= gameField.EdgeSize + gameField.CenterSize)
            {
                cellDestination.Value = new float2(2, cellPosition.Value.y);

                distance = (gameField.EdgeSize + gameField.CenterSize - 1) - cellPosition.Value.x;
            }
            else if (cellPosition.Value.y < gameField.EdgeSize)
            {
                cellDestination.Value = new float2(cellPosition.Value.x, 3);

                distance = gameField.EdgeSize - 1 - cellPosition.Value.y;
            }
            else if (cellPosition.Value.y >= gameField.EdgeSize + gameField.CenterSize)
            {
                cellDestination.Value = new float2(cellPosition.Value.x, 2);

                distance = (gameField.EdgeSize + gameField.CenterSize - 1) - cellPosition.Value.y;
            }
            else if (cellPosition.Value.y >= gameField.EdgeSize + gameField.CenterSize)
            {
                cellDestination.Value = new float2(cellPosition.Value.x, 2);
            }

            aspect.WorldDestination.TryAddOrGet(first).Value =
                CrossGrid.GetWorldPosition(
                    coordinates: cellDestination.Value,
                    field: gameField
                );

            return ref cellDestination;
        }
    }
}