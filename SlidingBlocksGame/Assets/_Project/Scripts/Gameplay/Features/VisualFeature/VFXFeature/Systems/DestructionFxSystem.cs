using _Project.Scripts.Gameplay.Features.DestructionFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Systems;
using _Project.Scripts.Gameplay.Features.VisualFeature.VFXFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.VFXFeature.Systems
{
    public class DestructionFxSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(DestroyViewRequest))]
            [Inc] public readonly EcsPool<DestructionVfxPrefab> DestructionVfx;

            [Inc] public readonly EcsPool<GameObjectConnect> GoConnects;
        }

        private class VfxAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsTagPool<PlayFxRequest> PlayFxRequest;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref readonly DestructionVfxPrefab vfxPrefab = ref aspect.DestructionVfx.Read(entity);

                ref readonly GameObjectConnect goConnect = ref aspect.GoConnects.Read(entity);
                
                EcsEntityConnect view = Object.Instantiate(
                    vfxPrefab.Value,
                    goConnect.Connect.transform.position, 
                    Quaternion.identity);

                entlong vfx = _world.NewEntityLong();

                view.Connect(vfx, applyTemplates: true);

                VfxAspect vfxAspect = _world.GetAspect<VfxAspect>();
                vfxAspect.PlayFxRequest.Add(vfx.ID);
            }
        }
    }
}