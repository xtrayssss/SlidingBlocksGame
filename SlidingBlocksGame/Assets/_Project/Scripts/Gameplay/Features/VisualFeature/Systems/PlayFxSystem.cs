using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.Systems
{
    public class PlayFxSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayFxRequest))]
            [Inc] public readonly EcsPool<ParticleSystemRef> Particles;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                Debug.Log("Play");
                aspect.Particles.Read(entity).Value.Play();
            }
        }
    }
}