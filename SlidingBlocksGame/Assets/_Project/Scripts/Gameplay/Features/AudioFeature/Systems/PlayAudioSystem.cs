using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Systems
{
    public class PlayAudioSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayAudioRequest))]
            [Inc] public readonly EcsPool<Audio> Audios;
            [Inc] public readonly EcsPool<AudioSourceRef> AudioSources;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                aspect.AudioSources.Read(entity).Value.PlayOneShot(aspect.Audios.Read(entity).Value);
            }
        }
    }
}