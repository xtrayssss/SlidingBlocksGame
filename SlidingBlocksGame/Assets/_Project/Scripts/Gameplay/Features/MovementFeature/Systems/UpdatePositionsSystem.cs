using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    public class UpdatePositionsSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(MovingMarker))]
            [Inc] public readonly EcsPool<CellPosition> CellPositions;
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;
            [Inc] public readonly EcsPool<ActiveGameField> ActiveGameFields;
        }

        private class GameFieldAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameField> GameFields;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                if (aspect.ActiveGameFields.Read(entity).Value.TryGetID(out int gameFieldID))
                {
                    GameFieldAspect gameFieldAspect = _world.GetAspect<GameFieldAspect>();

                    ref CellPosition cellPosition = ref aspect.CellPositions.Get(entity);
                    
                    cellPosition.Value =
                        GridUtils.GetCellPosition(
                            worldPosition: aspect.GameObjectConnects.Read(entity).Connect.transform.position,
                            gameField: in gameFieldAspect.GameFields.Read(gameFieldID));
                    
                    cellPosition.Fixed = GridUtils.GetCellPosition(
                        worldPosition: aspect.GameObjectConnects.Read(entity).Connect.transform.position,
                        gameField: in gameFieldAspect.GameFields.Read(gameFieldID));
                }
            }
        }
    }
}