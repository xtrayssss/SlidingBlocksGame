using System.Linq;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems;
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
            [IncImplicit(typeof(DestinationUnavailabilityCheckRequest))]
            [Inc] public readonly EcsPool<ObstacleCellPosition> ObstaclePositions;

            [Inc] public readonly EcsPool<TargetEntity> Targets;
        }

        private class AssignedGroupAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<AssignedGroup> Groups;
            [Inc] public readonly EcsPool<ObstacleCellPositions> Obstacles;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out SectionAspect aspect))
            {
                ref ObstacleCellPosition obstaclePosition = ref aspect.ObstaclePositions.Get(entity);

                AssignedGroupAspect assignedGroupAspect = _world.GetAspect<AssignedGroupAspect>();

                if (aspect.Targets.Read(entity).Value.TryGetID(out int blockID))
                {
                    if (assignedGroupAspect.Groups.Read(blockID).Value.TryGetID(out int groupID))
                    {
                        assignedGroupAspect.Obstacles.Get(groupID).Value.Add(obstaclePosition.Value);
                    }
                }
            }
        }
    }

    public class NearObstacleCalculationSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Units> Units;
            [Inc] public readonly EcsPool<ObstacleCellPositions> Obstacles;
            [Inc] public readonly EcsPool<MovementDirection> Directions;
            [Inc] public readonly EcsPool<ActiveGameField> ActiveGameFields;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref readonly var direction = ref aspect.Directions.Read(entity);

                if (math.all(direction.Value == new float2(1, 0)))
                {
                    aspect.Obstacles.Get(entity).Value =
                        aspect.Obstacles.Read(entity).Value.OrderBy(position => position.x).ToList();
                }
                else if (math.all(direction.Value == new float2(-1, 0)))
                {
                    aspect.Obstacles.Get(entity).Value =
                        aspect.Obstacles.Read(entity).Value.OrderBy(position => -position.x).ToList();
                }
                else if (math.all(direction.Value == new float2(0, 1)))
                {
                    aspect.Obstacles.Get(entity).Value =
                        aspect.Obstacles.Read(entity).Value.OrderBy(position => -position.y).ToList();
                }
                else if (math.all(direction.Value == new float2(0, -1)))
                {
                    aspect.Obstacles.Get(entity).Value =
                        aspect.Obstacles.Read(entity).Value.OrderBy(position => position.y).ToList();
                }

                ref readonly ActiveGameField activeGameField = ref aspect.ActiveGameFields.Read(entity);

                if (aspect.Obstacles.Get(entity).Value.Count != 0)
                {
                    foreach (entlong unit in aspect.Units.Read(entity).Value.Longs)
                    {
                        if (unit.TryGetID(out int unitID))
                        {
                            ref CellDestination cellDestination =
                                ref _world.GetPool<CellDestination>().TryAddOrGet(unitID);

                            cellDestination.Value =
                                _world.GetPool<CellPosition>().Read(unitID).Value + 2 * direction.Value;

                            _world.GetPool<WorldDestination>().TryAddOrGet(unitID).Value = CrossGrid.GetWorldPosition(
                                cellDestination.Value, _world.GetPool<GameField>().Read(activeGameField.Value.ID)
                            );
                        }
                    }
                }
            }
        }
    }
}