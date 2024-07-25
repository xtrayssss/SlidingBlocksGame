using System;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
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

                    view.ConnectWith(block, applyTemplates: true);
                }
            }
        }
    }
}