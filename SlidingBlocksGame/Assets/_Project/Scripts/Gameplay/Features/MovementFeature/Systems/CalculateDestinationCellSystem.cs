using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    public class CalculateDestinationCellSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CalculateDestinationCellRequest))]
            [Inc] public readonly EcsPool<WorldPosition> WorldPositions;
            [Inc] public readonly EcsPool<MovementDirection> Directions;
            [Opt] public readonly EcsPool<Destination> Destinations;

            [Opt] public readonly EcsPool<ActiveGameField> GameFields;
        }

        private class GameFieldAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameField> GameFields;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                if (aspect.GameFields.Read(entity).Value.TryGetID(out int id))
                {
                    GameFieldAspect gameFieldAspect = _world.GetAspect<GameFieldAspect>();

                    if (gameFieldAspect.IsMatches(id))
                    {
                        ref readonly GameField gameField = ref gameFieldAspect.GameFields.Read(id);

                        aspect.Destinations.TryAddOrGet(entity).Value = aspect.WorldPositions.Get(entity).Value +
                                                                        aspect.Directions.Read(entity).Value * (gameField.EdgeSize * (gameField.CellSize + gameField.Offset));
                    }
                }
            }
        }
    }
}