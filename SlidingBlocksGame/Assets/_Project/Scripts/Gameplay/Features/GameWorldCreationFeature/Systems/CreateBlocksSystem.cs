using System;
using System.Collections.Generic;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEditor;
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
        }

        private class BlockAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsPool<MovementDirection> Direction;
            [Opt] public readonly EcsPool<ActiveGameField> ActiveGameField;
            [Opt] public readonly EcsPool<AssignedGroup> AssignedGroup;
        }

        private class AssignedGroupAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsPool<MovementDirection> Directions;
            [Opt] public readonly EcsPool<ObstacleCellPositions> Obstacles;
            [Opt] public readonly EcsPool<Units> Units;
            [Opt] public readonly EcsPool<ActiveGameField> ActiveGameFields;
            [Opt] public readonly EcsPool<NearDistance> NearDistance;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref readonly GameField gameField = ref aspect.Fields.Read(entity);

                ReadOnlySpan<GameField.Unit> units = new ReadOnlySpan<GameField.Unit>(gameField.Units);

                BlockAspect blockAspect = _world.GetAspect<BlockAspect>();

                AssignedGroupAspect assignedGroupAspect = _world.GetAspect<AssignedGroupAspect>();
                
                int rightGroup = CreateGroup(new float2(1, 0), assignedGroupAspect, entity.ToEntityLong(_world));
                int leftGroup = CreateGroup(new float2(-1, 0), assignedGroupAspect, entity.ToEntityLong(_world));
                int upGroup = CreateGroup(new float2(0, 1), assignedGroupAspect, entity.ToEntityLong(_world));
                int downGroup = CreateGroup(new float2(0, -1), assignedGroupAspect, entity.ToEntityLong(_world));

                foreach (ref readonly GameField.Unit unit in units)
                {
                    entlong block = _world.NewEntityLong();

                    EcsEntityConnect view = Object.Instantiate(
                        original: unit.Prefab,
                        position: unit.Position,
                        rotation: quaternion.Euler(math.radians(unit.Rotation)));

                    _world.GetPool<CellPosition>().Add(block.ID).Value = unit.CellPosition;

                    view.ConnectWith(block, applyTemplates: true);

                    if (unit.CellPosition.x < gameField.EdgeSize)
                    {
                        blockAspect.Direction.Add(block.ID).Value = new float2(1, 0);

                        blockAspect.AssignedGroup.Add(block.ID).Value = rightGroup.ToEntityLong(_world);
                        
                        assignedGroupAspect.Units.Get(rightGroup).Value.Add(block.ID);
                    }
                    else if (unit.CellPosition.x >= gameField.EdgeSize + gameField.CenterSize)
                    {
                        blockAspect.Direction.Add(block.ID).Value = new float2(-1, 0);

                        blockAspect.AssignedGroup.Add(block.ID).Value = leftGroup.ToEntityLong(_world);

                        assignedGroupAspect.Units.Get(leftGroup).Value.Add(block.ID);
                    }
                    else if (unit.CellPosition.y < gameField.EdgeSize)
                    {
                        blockAspect.Direction.Add(block.ID).Value = new float2(0, 1);

                        blockAspect.AssignedGroup.Add(block.ID).Value = upGroup.ToEntityLong(_world);
                        
                        assignedGroupAspect.Units.Get(upGroup).Value.Add(block.ID);
                    }
                    else if (unit.CellPosition.y >= gameField.EdgeSize + gameField.CenterSize)
                    {
                        blockAspect.Direction.Add(block.ID).Value = new float2(0, -1);

                        blockAspect.AssignedGroup.Add(block.ID).Value = downGroup.ToEntityLong(_world);
                        
                        assignedGroupAspect.Units.Get(downGroup).Value.Add(block.ID);
                    }

                    blockAspect.ActiveGameField.Add(block.ID).Value = entity.ToEntityLong(_world);
                }
            }
        }

        private int CreateGroup(float2 direction, AssignedGroupAspect groupAspect, entlong gameField)
        {
            int group = _world.NewEntity();

            AssignedGroupAspect assignedGroupAspect = groupAspect;
            assignedGroupAspect.Directions.Add(group).Value = direction;
            assignedGroupAspect.Obstacles.Add(group).Value = new List<float2>();
            groupAspect.Units.Add(group).Value = EcsGroup.New(_world);
            assignedGroupAspect.ActiveGameFields.Add(group).Value = gameField;
            assignedGroupAspect.NearDistance.Add(group).Value = new float2(-1,- 1);

            return group;
        }
    }
}