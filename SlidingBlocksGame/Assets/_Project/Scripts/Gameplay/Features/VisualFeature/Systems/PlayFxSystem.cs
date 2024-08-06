using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.Systems
{
    public class PlayFxSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<ParticleSystemRef> Particles;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect)) 
                aspect.Particles.Read(entity).Value.Play();
        }
    }
}