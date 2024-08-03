using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    public class ObstaclePositionAdditionSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class SectionAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(DestinationUnavailabilityCheckRequest))] [Inc]
            public readonly EcsPool<ObstacleCellPosition> ObstaclePositions;

            [Inc] public readonly EcsPool<TargetEntity> Targets;
        }

        private class TargetAspect : EcsAspectAuto
        {
            [Inc] private readonly EcsTagPool<DestinationUnavailableMarker> _;

            [Inc] public readonly EcsPool<AssignedGroup> AssignedGroups;

            [Inc] public readonly EcsPool<Obstacle> Obstacles;
        }

        private class AssignedGroupAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<ObstacleCellPositions> Obstacles;
        }

        private class ObstacleAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<CellDestination> CellDestinations;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out SectionAspect aspect))
            {
                AssignedGroupAspect assignedGroupAspect = _world.GetAspect<AssignedGroupAspect>();

                TargetAspect targetAspect = _world.GetAspect<TargetAspect>();

                if (aspect.Targets.Read(entity).Value.TryGetID(out int targetID) &&
                    _world.GetTagPool<DestinationUnavailableMarker>().Has(targetID))
                {
                    if (targetAspect.AssignedGroups.Read(targetID).Value.TryGetID(out int groupID))
                    {
                        ObstacleAspect obstacleAspect = _world.GetAspect<ObstacleAspect>();

                        if (targetAspect.Obstacles.Read(targetID).Value.TryGetID(out int obstacleID) &&
                            obstacleAspect.IsMatches(obstacleID))
                        {
                            ref readonly var obstaclePosition = ref obstacleAspect.CellDestinations.Read(obstacleID);

                            if (assignedGroupAspect.Obstacles.Get(groupID).Value.Contains(obstaclePosition.Value))
                                continue;

                            Debug.Log(obstaclePosition.Value);

                            Debug.Log(targetID);
                            Debug.Log("adding");

                            assignedGroupAspect.Obstacles.Get(groupID).Value.Add(obstaclePosition.Value);
                        }
                    }
                }
            }
        }
    }

    public class NearObstacleCalculationSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class SectionAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(DestinationUnavailabilityCheckRequest))] [Inc]
            public readonly EcsPool<ObstacleCellPosition> ObstaclePositions;

            [Inc] public readonly EcsPool<TargetEntity> Targets;
        }

        private class TargetAspect : EcsAspectAuto
        {
            [Inc] private readonly EcsTagPool<DestinationUnavailableMarker> _;
            [Inc] public readonly EcsPool<AssignedGroup> AssignedGroups;
            [Inc] public readonly EcsPool<Obstacle> Obstacles;
            [Inc] public readonly EcsPool<CellPosition> CellPositions;

            [Opt] public readonly EcsTagPool<CalculateDestinationCellRequest> CalculationCellDestinationRequest;
        }

        private class AssignedGroupAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Units> Units;
            [Inc] public readonly EcsPool<ObstacleCellPositions> Obstacles;
            [Inc] public readonly EcsPool<MovementDirection> Directions;
            [Inc] public readonly EcsPool<ActiveGameField> ActiveGameFields;
            [Inc] public readonly EcsPool<NearDistance> NearDistances;
            [Opt] public readonly EcsTagPool<DetectionDistanceRequest> DetectionDistanceRequest;
        }

        private class ObstacleAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<CellDestination> CellDestinations;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out SectionAspect sectionAspect))
            {
                TargetAspect targetAspect = _world.GetAspect<TargetAspect>();

                if (sectionAspect.Targets.Read(entity).Value.TryGetID(out int targetID2))
                {
                    if (targetAspect.AssignedGroups.Read(targetID2).Value.TryGetID(out int groupID2))
                    {
                        _world.GetAspect<AssignedGroupAspect>().DetectionDistanceRequest.TryAdd(groupID2);
                    }
                }

                if (sectionAspect.Targets.Read(entity).Value.TryGetID(out int targetID) &&
                    _world.GetTagPool<DestinationUnavailableMarker>().Has(targetID))
                {
                    if (targetAspect.AssignedGroups.Read(targetID).Value.TryGetID(out int groupID))
                    {
                        if (targetAspect.Obstacles.Read(targetID).Value.TryGetID(out int obstacleID))
                        {
                            ObstacleAspect obstacleAspect = _world.GetAspect<ObstacleAspect>();

                            AssignedGroupAspect assignedGroupAspect = _world.GetAspect<AssignedGroupAspect>();

                            ref readonly var obstaclePosition = ref obstacleAspect.CellDestinations.Read(obstacleID);

                            ref ObstacleCellPositions obstacleCellPositions =
                                ref assignedGroupAspect.Obstacles.Get(groupID);

                            if (!obstacleCellPositions.Value.Contains(obstaclePosition.Value))
                                continue;

                            Debug.Log("'Near'");

                            ref readonly var direction = ref assignedGroupAspect.Directions.Read(groupID);

                            ref  Units units = ref assignedGroupAspect.Units.Get(groupID);

                            if (math.all(direction.Value == new float2(1, 0)))
                            {
                                Debug.Log("123")
                                    ;
                                obstacleCellPositions.Value =
                                    assignedGroupAspect.Obstacles.Read(groupID).Value.OrderBy(position => position.x)
                                        .ToList();

                                units.Value = units.Value.OrderBy(unit =>
                                {
                                    if (_world.GetEntityLong(unit).TryGetID(out int unitID))
                                    {
                                        return -_world.GetPool<CellPosition>().Read(unitID).Value.x;
                                    }

                                    return new float2(0, 0);
                                }).Select(x => x) as EcsGroup;
                            }
                            else if (math.all(direction.Value == new float2(-1, 0)))
                            {
                                obstacleCellPositions.Value =
                                    assignedGroupAspect.Obstacles.Read(groupID).Value.OrderBy(position => -position.x)
                                        .ToList();

                                units.Value = units.Value.OrderBy(unit =>
                                {
                                    if (_world.GetEntityLong(unit).TryGetID(out int unitID))
                                    {
                                        return -_world.GetPool<CellPosition>().Read(unitID).Value.x;
                                    }

                                    return new float2(0, 0);
                                }).Select(x => x) as EcsGroup;
                                
                            }
                            else if (math.all(direction.Value == new float2(0, 1)))
                            {
                                obstacleCellPositions.Value =
                                    assignedGroupAspect.Obstacles.Read(groupID).Value.OrderBy(position => -position.y)
                                        .ToList();
                                
                                units.Value = units.Value.OrderBy(unit =>
                                {
                                    if (_world.GetEntityLong(unit).TryGetID(out int unitID))
                                    {
                                        return -_world.GetPool<CellPosition>().Read(unitID).Value.y;
                                    }

                                    return new float2(0, 0);
                                }).Select(x => x) as EcsGroup;
                            }
                            else if (math.all(direction.Value == new float2(0, -1)))
                            {
                                obstacleCellPositions.Value =
                                    assignedGroupAspect.Obstacles.Read(groupID).Value.OrderBy(position => position.y)
                                        .ToList();
                                
                                units.Value = units.Value.OrderBy(unit =>
                                {
                                    if (_world.GetEntityLong(unit).TryGetID(out int unitID))
                                    {
                                        return -_world.GetPool<CellPosition>().Read(unitID).Value.y;
                                    }

                                    return new float2(0, 0);
                                }).Select(x => x) as EcsGroup;
                            }

                            foreach (var obstacle in assignedGroupAspect.Obstacles.Read(groupID).Value)
                            {
                                Debug.Log(obstacle);
                            }

                            if (obstacleCellPositions.Value.Count != 0)
                            {
                                foreach (var unit in units.Value.Longs)
                                {
                                    if (unit.TryGetID(out int unitID))
                                    {
                                        float2 nearObstacle = obstacleCellPositions.Value.First();

                                        Debug.Log(nearObstacle);

                                        float2 distance =
                                            math.abs(nearObstacle - targetAspect.CellPositions.Read(unitID).Value);

                                        Debug.Log(distance);

                                        bool any = math.any(
                                            direction.Value * assignedGroupAspect.NearDistances.Get(groupID).Value >
                                            direction.Value * distance);

                                        if (any || math.all(assignedGroupAspect.NearDistances.Get(groupID).Value ==
                                                            new float2(-1, -1)))
                                        {
                                            Debug.Log("12");
                                            assignedGroupAspect.NearDistances.Get(groupID).Value = distance;

                                            targetAspect.CalculationCellDestinationRequest.Add(targetID);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}