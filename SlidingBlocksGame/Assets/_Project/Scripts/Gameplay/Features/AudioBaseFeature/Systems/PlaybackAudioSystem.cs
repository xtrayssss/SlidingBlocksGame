using _Project.Scripts.Gameplay.Features.AudioBaseFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.AudioBaseFeature.Systems
{
    public class PlaybackAudioSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class PlayOneShotAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayOneShotAudioRequest))]
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

                Debug.Log("AUDIO RESTARTED");
            }

            foreach (int entity in _world.Where(out PlayOneShotAspect aspect))
                aspect.AudioSources.Read(entity).Value.PlayOneShot(aspect.Audios.Read(entity).Value);

            foreach (int entity in _world.Where(out PlayAspect aspect))
            {
                ref AudioSourceRef audioSource = ref aspect.AudioSources.Get(entity);

                audioSource.Value.clip = aspect.Audios.Read(entity).Value;

                audioSource.Value.Play();

                Debug.Log("AUDIO PLAY");
            }
        }
    }
}