using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Systems
{
    public class PlayAudioSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class OneShotAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayAudioRequest))]
            [ExcImplicit(typeof(AudioLoopMarker))]
            [Inc] public readonly EcsPool<Audio> Audios;

            [Inc] public readonly EcsPool<AudioSourceRef> AudioSources;
        }

        private class PlayAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayAudioRequest))]
            [IncImplicit(typeof(AudioLoopMarker))]
            [Inc] public readonly EcsPool<Audio> Audios;

            [Inc] public readonly EcsPool<AudioSourceRef> AudioSources;
        }

        private class StopAudioAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(StopAudioRequest))]
            [Inc] public readonly EcsPool<AudioSourceRef> AudioSources;
        }

        private class RestartAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(RestartAudioRequest))]
            [Inc] public readonly EcsPool<AudioSourceRef> AudioSources;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out StopAudioAspect aspect))
                aspect.AudioSources.Get(entity).Value.Stop();

            foreach (int entity in _world.Where(out RestartAspect aspect))
            {
                ref AudioSourceRef audioSource = ref aspect.AudioSources.Get(entity);
                audioSource.Value.Stop();
                audioSource.Value.Play();
            }

            foreach (int entity in _world.Where(out OneShotAspect aspect))
                aspect.AudioSources.Read(entity).Value.PlayOneShot(aspect.Audios.Read(entity).Value);

            foreach (int entity in _world.Where(out PlayAspect aspect))
            {
                ref AudioSourceRef audioSource = ref aspect.AudioSources.Get(entity);

                audioSource.Value.clip = aspect.Audios.Read(entity).Value;

                audioSource.Value.Play();
            }
        }
    }
}