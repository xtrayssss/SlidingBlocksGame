using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Systems
{
    public class WorldPositionSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameObjectConnect> Transforms;
            [Inc] public readonly EcsPool<WorldPosition> WorldPositions;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect)) 
                aspect.WorldPositions.Get(entity).Value = aspect.Transforms.Get(entity).Connect.transform.position;
        }
    }
}