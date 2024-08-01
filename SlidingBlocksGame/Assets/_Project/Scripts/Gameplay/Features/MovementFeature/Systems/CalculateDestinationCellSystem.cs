using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    public class CalculateDestinationCellSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CalculateDestinationCellRequest))]
            [Inc] public readonly EcsPool<WorldPosition> WorldPositions;

            [Inc] public readonly EcsPool<CellPosition> CellPositions;
            [Inc] public readonly EcsPool<MovementDirection> Directions;
            [Inc] public readonly EcsPool<ActiveGameField> AspectGameFields;

            [Exc] public readonly EcsPool<WorldDestination> WorldDestination;
            [Exc] public readonly EcsPool<CellDestination> CellDestination;
        }

        private class GameFieldAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameField> GameFields;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
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
        }
    }
}