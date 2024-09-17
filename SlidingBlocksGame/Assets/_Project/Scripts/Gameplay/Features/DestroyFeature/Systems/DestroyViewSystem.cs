using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.DestroyFeature.Systems
{
    public class DestroyViewSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(DestroyViewRequest))]
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;

            [Opt] public readonly EcsTagPool<ViewDestroyedEvent> ViewDestroyedEvent;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                Object.Destroy(aspect.GameObjectConnects.Read(entity).Connect.gameObject);

                if (entity.ToEntityLong(_world).IsAlive) 
                    aspect.ViewDestroyedEvent.Add(entity);
            }
        }
    }
}