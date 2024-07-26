using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    public class ChainingBlocksSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class BlockAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(BlockTag))]
            [Inc] public readonly EcsPool<MovementDirection> Directions;

            [Opt] public readonly EcsTagPool<MovementCommand> Commands;
        }

        private class ClickAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ClickTag))] [Inc] public readonly EcsPool<WorldPosition> WorldPositions;
        }

        private class GameAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameField> Fields;
        }

        public void Run()
        {
            foreach (int click in _world.Where(out ClickAspect clickAspect))
            {
                ref readonly WorldPosition clickPosition = ref clickAspect.WorldPositions.Read(click);

                foreach (int game in _world.Where(out GameAspect gameAspect))
                {
                    ref readonly GameField gameField = ref gameAspect.Fields.Read(game);

                    float2 cellPosition = WorldToGridPosition(clickPosition.Value, gameField);

                    EcsGroup blocks = EcsGroup.New(_world);
                    
                    foreach (int block in _world.Where(out BlockAspect blockAspect))
                    {
                        ref readonly MovementDirection direction = ref blockAspect.Directions.Read(block);

                        bool val = false;

                        if (cellPosition.x < gameField.EdgeSize && math.all(direction.Value == new float3(1, 0, 0)))
                        {
                            val = true;
                        }
                        else if (cellPosition.x >= gameField.EdgeSize + gameField.CenterSize &&
                                 math.all(direction.Value == new float3(-1, 0, 0)))
                        {
                            val = true;
                        }
                        else if (cellPosition.y < gameField.EdgeSize &&
                                 math.all(direction.Value == new float3(0, 0, 1)))
                        {
                            val = true;
                        }
                        else if (cellPosition.y >= gameField.EdgeSize + gameField.CenterSize &&
                                 math.all(direction.Value == new float3(0, 0, -1)))
                        {
                            val = true;
                        }
                        
                        if (val)
                        {
                            Debug.Log("chained");
                            blocks.Add(block);
                        }
                    }

                    Debug.Log(blocks.Count);
                    
                    int chain = _world.NewEntity();
                    
                    _world.GetPool<Chain>().Add(chain).Value = blocks;
                    _world.GetPool<ChainMovementMarker>().Add(chain);
                    _world.GetPool<ChainMovementCooldown>().Add(chain).Duration = 0.5f;
                    _world.GetPool<DeleteEntityCommand>().Add(chain);
                }
            }
        }

        private float2 WorldToGridPosition(float3 worldPosition, GameField field)
        {
            int x = Mathf.FloorToInt((worldPosition.x - field.OriginPosition.x) / (field.CellSize + field.Offset));
            int z = Mathf.FloorToInt((worldPosition.z - field.OriginPosition.z) / (field.CellSize + field.Offset));

            return new float2(x, z);
        }
    }
}