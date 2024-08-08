using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Systems
{
    public class CheckAnimalWithinCenterSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CellOccupancyMarker))]
            [ExcImplicit(typeof(WithinCenterMarker))] 
            [Inc] public readonly EcsPool<CellDestination> CellDestinations;

            [Inc] public readonly EcsPool<ActiveGameField> ActiveGameFields;

            [Opt] public readonly EcsTagPool<WithinCenterMarker> WithinCenterMarker;
        }

        private class GameFieldAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameField> GameFields;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref readonly CellDestination cellDestination = ref aspect.CellDestinations.Read(entity);

                GameFieldAspect gameFieldAspect = _world.GetAspect<GameFieldAspect>();

                if (aspect.ActiveGameFields.Read(entity).Value.TryGetID(out int gameFieldID))
                {
                    if (gameFieldAspect.IsMatches(gameFieldID))
                    {
                        ref readonly GameField gameField = ref gameFieldAspect.GameFields.Read(gameFieldID);

                        if (cellDestination.Value.x >= gameField.EdgeSize &&
                            cellDestination.Value.x < gameField.EdgeSize + gameField.CenterSize &&
                            cellDestination.Value.y >= gameField.EdgeSize &&
                            cellDestination.Value.y < gameField.EdgeSize + gameField.CenterSize)
                        {
                            aspect.WithinCenterMarker.Add(entity);
                        }
                    }
                }
            }
        }
    }
}