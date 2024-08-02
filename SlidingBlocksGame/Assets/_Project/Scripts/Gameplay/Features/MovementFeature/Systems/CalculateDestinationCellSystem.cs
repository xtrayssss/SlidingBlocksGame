using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using Unity.VisualScripting;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    public class CalculateDestinationCellSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class MaxDistanceAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CalculateDestinationCellRequest))]
            [IncImplicit(typeof(CalculationMaxDistanceRequest))]
            [Inc] public readonly EcsPool<CellPosition> CellPositions;

            [Inc] public readonly EcsPool<MovementDirection> Directions;
            [Inc] public readonly EcsPool<ActiveGameField> AspectGameFields;

            [Exc] public readonly EcsPool<WorldDestination> WorldDestination;
            [Exc] public readonly EcsPool<CellDestination> CellDestination;
        }

        private class ObstacleDistanceAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CalculateDestinationCellRequest))]
            [IncImplicit(typeof(CalculateObstacleDistanceRequest))]
            [Inc] public readonly EcsPool<CellPosition> CellPositions;

            [Inc] public readonly EcsPool<AssignedGroup> AssignedGroups;
            [Inc] public readonly EcsPool<ActiveGameField> AspectGameFields;

            [Exc] public readonly EcsPool<WorldDestination> WorldDestination;
            [Exc] public readonly EcsPool<CellDestination> CellDestination;
        }

        private class AssignedGroupAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<MovementDirection> Directions;
            [Inc] public readonly EcsPool<NearDistance> NearDistances;
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
                    ref readonly GameField gameField = ref gameFieldAspect.GameFields.Read(activeGameFieldID);
                    ref readonly CellPosition cellPosition = ref aspect.CellPositions.Read(entity);

                    ref CellDestination cellDestination = ref aspect.CellDestination.Add(entity);

                    cellDestination.Value =
                        cellPosition.Value + gameField.EdgeSize * aspect.Directions.Read(entity).Value;

                    aspect.WorldDestination.Add(entity).Value =
                        CrossGrid.GetWorldPosition(
                            coordinates: cellDestination.Value,
                            field: gameField
                        );
                }
            }

            foreach (int entity in _world.Where(out ObstacleDistanceAspect aspect))
            {
                ActiveGameField activeGameField = aspect.AspectGameFields.Read(entity);
                GameFieldAspect gameFieldAspect = _world.GetAspect<GameFieldAspect>();

                if (activeGameField.Value.TryGetID(out int activeGameFieldID))
                {
                    ref readonly GameField gameField = ref gameFieldAspect.GameFields.Read(activeGameFieldID);
                    ref readonly CellPosition cellPosition = ref aspect.CellPositions.Read(entity);

                    ref CellDestination cellDestination = ref aspect.CellDestination.Add(entity);

                    AssignedGroupAspect assignedGroupAspect = _world.GetAspect<AssignedGroupAspect>();

                    if (!aspect.AssignedGroups.Read(entity).Value.TryGetID(out int assignedGroupID) ||
                        !assignedGroupAspect.IsMatches(assignedGroupID))
                        continue;

                    ref readonly NearDistance nearDistance =
                        ref assignedGroupAspect.NearDistances.Read(assignedGroupID);

                    cellDestination.Value =
                        cellPosition.Value + nearDistance.Value *
                        assignedGroupAspect.Directions.Read(assignedGroupID).Value;
                    
                    cellDestination.Value -=
                        assignedGroupAspect.Directions.Read(assignedGroupID).Value * new float2(1, 1);
                    
                    aspect.WorldDestination.Add(entity).Value =
                        CrossGrid.GetWorldPosition(
                            coordinates: cellDestination.Value,
                            field: gameField
                        );
                }
            }
        }
    }
}