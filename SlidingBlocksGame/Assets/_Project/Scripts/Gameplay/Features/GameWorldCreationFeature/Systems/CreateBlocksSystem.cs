using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor;
using UnityEngine;
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

        private class AnimalAspect : EcsAspectAuto
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

                Span<GameField.Unit> units = new Span<GameField.Unit>(gameField.Units);

                AnimalAspect animalAspect = _world.GetAspect<AnimalAspect>();

                AssignedGroupAspect assignedGroupAspect = _world.GetAspect<AssignedGroupAspect>();
                
                int rightGroup = CreateGroup(new float2(1, 0), assignedGroupAspect, entity.ToEntityLong(_world));
                int leftGroup = CreateGroup(new float2(-1, 0), assignedGroupAspect, entity.ToEntityLong(_world));
                int upGroup = CreateGroup(new float2(0, 1), assignedGroupAspect, entity.ToEntityLong(_world));
                int downGroup = CreateGroup(new float2(0, -1), assignedGroupAspect, entity.ToEntityLong(_world));

                foreach (ref GameField.Unit unit in units)
                {
                    entlong animal = _world.NewEntityLong();

                    EcsEntityConnect view = Object.Instantiate(
                        original: unit.Prefab,
                        position: unit.Position + new float3(gameField.UnitCellTopOffset),
                        rotation: quaternion.Euler(math.radians(unit.Rotation)));

                    unit.View = view.gameObject;

                    view.ConnectWith(animal, applyTemplates: true);

                    ScaleRenderer(animal);

                    _world.GetPool<CellPosition>().Add(animal.ID).Value = unit.CellPosition;

                    HandleDirection(unit, gameField, animalAspect, animal, rightGroup, assignedGroupAspect, leftGroup, upGroup, downGroup);

                    animalAspect.ActiveGameField.Add(animal.ID).Value = entity.ToEntityLong(_world);
                }
            }
        }

        private void HandleDirection(GameField.Unit unit, GameField gameField, AnimalAspect animalAspect, entlong animal, int rightGroup,
            AssignedGroupAspect assignedGroupAspect, int leftGroup, int upGroup, int downGroup)
        {
            if (unit.CellPosition.x < gameField.EdgeSize)
            {
                animalAspect.Direction.Add(animal.ID).Value = new float2(1, 0);

                animalAspect.AssignedGroup.Add(animal.ID).Value = rightGroup.ToEntityLong(_world);
                        
                assignedGroupAspect.Units.Get(rightGroup).Value.Add(animal.ID);
            }
            else if (unit.CellPosition.x >= gameField.EdgeSize + gameField.CenterSize)
            {
                animalAspect.Direction.Add(animal.ID).Value = new float2(-1, 0);

                animalAspect.AssignedGroup.Add(animal.ID).Value = leftGroup.ToEntityLong(_world);

                assignedGroupAspect.Units.Get(leftGroup).Value.Add(animal.ID);
            }
            else if (unit.CellPosition.y < gameField.EdgeSize)
            {
                animalAspect.Direction.Add(animal.ID).Value = new float2(0, 1);

                animalAspect.AssignedGroup.Add(animal.ID).Value = upGroup.ToEntityLong(_world);
                        
                assignedGroupAspect.Units.Get(upGroup).Value.Add(animal.ID);
            }
            else if (unit.CellPosition.y >= gameField.EdgeSize + gameField.CenterSize)
            {
                animalAspect.Direction.Add(animal.ID).Value = new float2(0, -1);

                animalAspect.AssignedGroup.Add(animal.ID).Value = downGroup.ToEntityLong(_world);
                        
                assignedGroupAspect.Units.Get(downGroup).Value.Add(animal.ID);
            }
        }

        private void ScaleRenderer(entlong animal)
        {
            ref RendererRef renderer = ref _world.GetPool<RendererRef>().Get(animal.ID);
            
            var meshFilter =  renderer.Value.GetComponent<MeshFilter>();
            
            float size = 1 / meshFilter.mesh.bounds.size.x;

            renderer.Value.transform.localScale = new Vector3(size, size, size);
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
        
        Bounds TransformBounds(Transform transform, Bounds localBounds)
        {
            Vector3 center = transform.TransformPoint(localBounds.center);
            Vector3 extents = localBounds.extents;
            Vector3 worldExtents = transform.TransformVector(extents);

            Bounds worldBounds = new Bounds(center, Vector3.zero);
            worldBounds.Encapsulate(center + worldExtents);
            worldBounds.Encapsulate(center - worldExtents);

            return worldBounds;
        }
    }
}