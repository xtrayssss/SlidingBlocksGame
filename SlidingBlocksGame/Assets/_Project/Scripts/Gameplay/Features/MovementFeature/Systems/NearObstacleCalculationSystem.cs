using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;
using Unity.Collections.LowLevel.Unsafe;
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

            public bool CustomIsMatches(int entity) =>
                AssignedGroups.Has(entity) &&
                Obstacles.Has(entity) &&
                CellPositions.Has(entity) &&
                World.GetPool<DestinationUnavailableMarker>().Has(entity);
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
                Debug.Log(123);

                if (sectionAspect.Targets.Read(entity).Value.TryGetID(out int targetID2))
                {
                    if (targetAspect.AssignedGroups.Read(targetID2).Value.TryGetID(out int groupID2))
                    {
                        AssignedGroupAspect assignedGroupAspect = _world.GetAspect<AssignedGroupAspect>();

                        assignedGroupAspect.DetectionDistanceRequest.TryAdd(groupID2);
                    }
                }

                if (sectionAspect.Targets.Read(entity).Value.TryGetID(out int targetID) &&
                    _world.GetTagPool<DestinationUnavailableMarker>().Has(targetID))
                {
                    if (targetAspect.AssignedGroups.Read(targetID).Value.TryGetID(out int groupID))
                    {
                        ObstacleAspect obstacleAspect = _world.GetAspect<ObstacleAspect>();

                        AssignedGroupAspect assignedGroupAspect = _world.GetAspect<AssignedGroupAspect>();

                        Debug.Log("'Near'");

                        ref readonly var direction = ref assignedGroupAspect.Directions.Read(groupID);

                        ref Units units = ref assignedGroupAspect.Units.Get(groupID);

                        if (math.all(direction.Value == new float2(1, 0)))
                        {
                            int unitWithObstacle = units.Value.ToList().OrderBy(x =>
                            {
                                if (_world.GetEntityLong(x).TryGetID(out int unitID))
                                {
                                    targetAspect.CalculationCellDestinationRequest.TryAdd(unitID);

                                    if (targetAspect.CustomIsMatches(unitID) && targetAspect.Obstacles.Read(unitID)
                                            .Value.TryGetID(out int obstacleID))
                                    {
                                        return obstacleAspect.CellDestinations.Read(obstacleID).Value.x;
                                    }
                                }

                                return 0;
                            }).First();

                            if (_world.GetEntityLong(unitWithObstacle).TryGetID(out int unitObstacleID))
                            {
                                if (targetAspect.Obstacles.Read(unitObstacleID).Value.TryGetID(out int obstacleID))
                                {
                                    assignedGroupAspect.NearDistances.Get(groupID).Value =
                                        math.abs(_world.GetPool<CellDestination>().Read(obstacleID).Value -
                                                 _world.GetPool<CellPosition>().Read(unitObstacleID).Value);
                                }
                            }
                        }
                        else if (math.all(direction.Value == new float2(-1, 0)))
                        {
                            int unitWithObstacle = units.Value.ToList().OrderBy(x =>
                            {
                                if (_world.GetEntityLong(x).TryGetID(out int unitID))
                                {
                                    targetAspect.CalculationCellDestinationRequest.TryAdd(unitID);

                                    if (targetAspect.CustomIsMatches(unitID) && targetAspect.Obstacles.Read(unitID)
                                            .Value
                                            .TryGetID(out int obstacleID))
                                    {
                                        Debug.Log(obstacleID);
                                        return -obstacleAspect.CellDestinations.Read(obstacleID).Value.x;
                                    }
                                }


                                return 0;
                            }).First();

                            if (_world.GetEntityLong(unitWithObstacle).TryGetID(out int unitObstacleID))
                            {
                                if (targetAspect.Obstacles.Read(unitObstacleID).Value.TryGetID(out int obstacleID))
                                {
                                    assignedGroupAspect.NearDistances.Get(groupID).Value =
                                        math.abs(_world.GetPool<CellDestination>().Read(obstacleID).Value -
                                                 _world.GetPool<CellPosition>().Read(unitObstacleID).Value);
                                }
                            }
                        }

                        else if (math.all(direction.Value == new float2(0, 1)))
                        {
                            int unitWithObstacle = units.Value.ToList().OrderBy(x =>
                            {
                                if (_world.GetEntityLong(x).TryGetID(out int unitID))
                                {
                                    targetAspect.CalculationCellDestinationRequest.TryAdd(unitID);
                                    if (targetAspect.CustomIsMatches(unitID) && targetAspect.Obstacles.Read(unitID)
                                            .Value.TryGetID(out int obstacleID))
                                    {
                                        return obstacleAspect.CellDestinations.Read(obstacleID).Value.y;
                                    }
                                }


                                return 0;
                            }).First();

                            if (_world.GetEntityLong(unitWithObstacle).TryGetID(out int unitObstacleID))
                            {
                                if (targetAspect.Obstacles.Read(unitObstacleID).Value.TryGetID(out int obstacleID))
                                {
                                    assignedGroupAspect.NearDistances.Get(groupID).Value =
                                        math.abs(_world.GetPool<CellDestination>().Read(obstacleID).Value -
                                                 _world.GetPool<CellPosition>().Read(unitObstacleID).Value);
                                }
                            }
                        }
                        else if (math.all(direction.Value == new float2(0, -1)))
                        {
                            int unitWithObstacle = units.Value.ToList().OrderBy(x =>
                            {
                                if (_world.GetEntityLong(x).TryGetID(out int unitID))
                                {
                                    targetAspect.CalculationCellDestinationRequest.TryAdd(unitID);
                                    if (targetAspect.CustomIsMatches(unitID) && targetAspect.Obstacles.Read(unitID)
                                            .Value.TryGetID(out int obstacleID))
                                    {
                                        return -obstacleAspect.CellDestinations.Read(obstacleID).Value.y;
                                    }
                                }


                                return 0;
                            }).First();

                            if (_world.GetEntityLong(unitWithObstacle).TryGetID(out int unitObstacleID))
                            {
                                if (targetAspect.Obstacles.Read(unitObstacleID).Value.TryGetID(out int obstacleID))
                                {
                                    assignedGroupAspect.NearDistances.Get(groupID).Value =
                                        math.abs(_world.GetPool<CellDestination>().Read(obstacleID).Value -
                                                 _world.GetPool<CellPosition>().Read(unitObstacleID).Value);
                                }
                            }
                        }


                        // float2 nearObstacle = obstacleCellPositions.Value.First();
                        //
                        // Debug.Log(nearObstacle);

                        // float2 distance =
                        //     math.abs(nearObstacle - targetAspect.CellPositions.Read(unitID).Value);
                        //
                        // Debug.Log(distance);
                        //
                        // bool any = math.any(
                        //     direction.Value * assignedGroupAspect.NearDistances.Get(groupID).Value >
                        //     direction.Value * distance);
                        //
                        // if (any || math.all(assignedGroupAspect.NearDistances.Get(groupID).Value ==
                        //                     new float2(-1, -1)))
                        // {
                        //     Debug.Log("12");
                        //     assignedGroupAspect.NearDistances.Get(groupID).Value = distance;
                        //
                        //     targetAspect.CalculationCellDestinationRequest.Add(targetID);
                        // }
                    }
                }
                // }
                //}
            }
        }
    }
}