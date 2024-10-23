using _Project.Scripts.Gameplay.Features.AudioFeature.Behaviours;
using _Project.Scripts.Gameplay.Features.GameAudioFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameAudioFeature.Systems
{
    public class SwitchAudioSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AudioSettingsUpdatedEvent))]
            [Inc] public readonly EcsPool<AudioSettings> AudioSettings;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref readonly AudioSettings audioSettings = ref aspect.AudioSettings.Read(entity);

                if (audioSettings.SoundIsOn)
                    GameAudio.Instance.Sfx.Mixer.audioMixer.SetFloat(AudioConstants.SFX_VOLUME, 0);
                else
                    GameAudio.Instance.Sfx.Mixer.audioMixer.SetFloat(AudioConstants.SFX_VOLUME, -80);

                if (audioSettings.MusicIsOn)
                    GameAudio.Instance.Music.Mixer.audioMixer.SetFloat(AudioConstants.MUSIC_VOLUME, 0);
                else
                    GameAudio.Instance.Music.Mixer.audioMixer.SetFloat(AudioConstants.MUSIC_VOLUME, -80);
            }
        }
    }
}