using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Utils;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    public class SyncPositionsSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class MovableAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(MovingMarker))]
            [Inc] public readonly EcsPool<WorldPosition> WorldPositions;

            [Inc] public readonly EcsPool<GameObjectConnect> GoConnects;
        }

        private class GameFieldMovableAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(MovingMarker))]
            [Inc] public readonly EcsPool<ActiveGameField> ActiveGameFields;

            [Inc] public readonly EcsPool<CellPosition> CellPositions;
        }

        private class GameFieldAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameField> GameFields;
        }

        public void Run()
        {
            foreach (int movable in _world.Where(out MovableAspect movableAspect))
            {
                ref WorldPosition worldPosition = ref movableAspect.WorldPositions.Get(movable);
                ref GameObjectConnect goConnect = ref movableAspect.GoConnects.Get(movable);
                worldPosition.Value = goConnect.Connect.transform.position;

                GameFieldMovableAspect gameFieldMovableAspect = _world.GetAspect<GameFieldMovableAspect>();

                if (!gameFieldMovableAspect.IsMatches(movable))
                    continue;

                if (gameFieldMovableAspect.ActiveGameFields.Read(movable).Value.TryGetID(out int gameFieldID))
                {
                    GameFieldAspect gameFieldAspect = _world.GetAspect<GameFieldAspect>();

                    ref CellPosition cellPosition = ref gameFieldMovableAspect.CellPositions.Get(movable);

                    ref readonly GameField gameField = ref gameFieldAspect.GameFields.Read(gameFieldID);

                    cellPosition.Value =
                        GridUtils.GetCellPosition(
                            worldPosition: goConnect.Connect.transform.position,
                            new GridUtils.Grid
                            {
                                CellSize = gameField.CellSize,
                                OriginPosition = gameField.OriginPosition,
                                Offset = gameField.Offset
                            });
                }
            }
        }
    }
}