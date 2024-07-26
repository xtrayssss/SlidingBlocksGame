using System;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class CreateBlocksSystem : IEcsRun
    {
        [EcsInject] private EcsWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CreateBlocksRequest))] [Inc]
            public readonly EcsPool<GameField> Fields;

            [Opt] public readonly EcsPool<MovementDirection> Directions;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref readonly GameField field = ref aspect.Fields.Read(entity);

                ReadOnlySpan<GameField.Unit> units = new ReadOnlySpan<GameField.Unit>(field.Units);

                foreach (ref readonly GameField.Unit unit in units)
                {
                    entlong block = _world.NewEntityLong();

                    EcsEntityConnect view = Object.Instantiate(
                        original: unit.Prefab,
                        position: unit.Position,
                        rotation: quaternion.Euler(math.radians(unit.Rotation)));

                    _world.GetPool<CellPosition>().Add(block.ID).Value = unit.CellPosition;

                    view.ConnectWith(block, applyTemplates: true);

                    if (unit.CellPosition.x < field.EdgeSize)
                    {
                        aspect.Directions.Add(block.ID).Value = new float3(1, 0, 0);
                    }
                    else if (unit.CellPosition.x >= field.EdgeSize + field.CenterSize)
                    {
                        aspect.Directions.Add(block.ID).Value = new float3(-1, 0, 0);
                    }
                    else if (unit.CellPosition.y < field.EdgeSize)
                    {
                        aspect.Directions.Add(block.ID).Value = new float3(0, 0, 1);
                    }
                    else if (unit.CellPosition.y >= field.EdgeSize + field.CenterSize)
                    {
                        aspect.Directions.Add(block.ID).Value = new float3(0, 0, -1);
                    }

                    _world.GetPool<CalculateDestinationCellRequest>().Add(block.ID);
                    _world.GetPool<ActiveGameField>().Add(block.ID).Value = entity.ToEntityLong(_world);
                }
            }
        }
    }
}