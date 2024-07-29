using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;

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

            [Opt] public readonly EcsPool<WorldDestination> WorldDestination;
            [Opt] public readonly EcsPool<CellDestination> CellDestination;
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

                        float3 direction = new float3(aspect.Directions.Read(entity).Value.x, 0,
                            aspect.Directions.Read(entity).Value.y);
                        
                        ref CellDestination cellDestination = ref  aspect.CellDestination.TryAddOrGet(entity);
                        ref CellPosition cellPosition = ref aspect.CellPositions.Get(entity);
                        
                        int centerX = gameField.EdgeSize + gameField.CenterSize / 2;
                        int centerZ = gameField.EdgeSize + gameField.CenterSize / 2;

                        int newX = (int)cellPosition.Value.x;
                        int newZ = (int)cellPosition.Value.y;

                        Debug.Log(centerX);
                        if (cellPosition.Value.x < gameField.EdgeSize)
                        {
                            newX = centerX;
                        }
                        else if (cellPosition.Value.x >= gameField.EdgeSize + gameField.CenterSize)
                        {
                            newX = centerX - 1;
                        }

                        if (cellPosition.Value.y >= gameField.EdgeSize + gameField.CenterSize)
                        {
                            newZ = centerZ - 1;
                        }
                        else if (cellPosition.Value.y < gameField.EdgeSize)
                        {
                            newZ = centerZ;
                        }

                        aspect.WorldDestination.TryAddOrGet(entity).Value = CrossGrid.GetWorldPosition(new float2(newX, newZ), gameField)/*aspect.WorldPositions.Get(entity).Value +
                                                                            direction * (gameField.EdgeSize *
                                                                                (gameField.CellSize +
                                                                                    gameField.Offset))*/;

                        cellDestination.Value = new float2(newX, newZ) /*cellPosition.Value +
                                                aspect.Directions.Read(entity).Value *
                                                gameField.EdgeSize*/;
                    }
                }
            }
        }
    }
}