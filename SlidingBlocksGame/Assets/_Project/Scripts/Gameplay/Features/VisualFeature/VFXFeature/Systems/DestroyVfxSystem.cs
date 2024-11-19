using _Project.Scripts.Gameplay.Features.DestructionFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.VFXFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.VFXFeature.Systems
{
    public class DestroyVfxSystem : IEcsRun
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