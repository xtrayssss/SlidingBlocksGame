using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.DestructionFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.Systems
{
    public class DestroyFxRequestSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(DestroyAfterPlaybackMarker))]
            [Inc] public readonly EcsPool<ParticleSystemRef> Particles;
            
            [Opt] public readonly EcsTagPool<DestroyViewRequest> DestroyViewRequests;
            [Opt] public readonly EcsTagPool<DeleteEntityRequest> DeleteEntityCommand;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref readonly ParticleSystemRef particleSystem = ref aspect.Particles.Read(entity);
                
                if (!particleSystem.Value.isPlaying)
                {
                    aspect.DestroyViewRequests.Add(entity);
                    aspect.DeleteEntityCommand.Add(entity);
                }
            }
        }
    }
}