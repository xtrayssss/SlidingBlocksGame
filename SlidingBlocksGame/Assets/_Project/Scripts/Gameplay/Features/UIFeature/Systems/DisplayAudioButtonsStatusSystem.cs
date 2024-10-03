using _Project.Scripts.Gameplay.Features.AudioBaseFeature;
using _Project.Scripts.Gameplay.Features.AudioBaseFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public struct UpdateGameAudioRequest : IEcsComponent
    {
        public bool IsMusicOn;
        public bool IsSoundOn;
    }

    public class DisplayAudioButtonsStatusSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;


        private class SettingsPopupAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(SettingsPopupTag))]
            [IncImplicit(typeof(GameAudioUpdatedEvent))]
            [Inc] public readonly EcsPool<AudioButtonsStatus> AudioButtonsStatus;
        }

        public void Run()
        {
            foreach (int popup in _world.Where(out SettingsPopupAspect settingsPopupAspect))
            {
                ref AudioButtonsStatus audioButtonsStatus = ref settingsPopupAspect.AudioButtonsStatus.Get(popup);

                if (audioButtonsStatus.SoundIsOn)
                {
                    GameAudio.Instance.Sfx.Mixer.audioMixer.SetFloat("SFXVolume", 0);

                    audioButtonsStatus.SoundOff.gameObject.SetActive(false);
                    audioButtonsStatus.SoundOn.gameObject.SetActive(true);
                }
                else
                {
                    GameAudio.Instance.Sfx.Mixer.audioMixer.SetFloat("SFXVolume", -80);

                    audioButtonsStatus.SoundOn.gameObject.SetActive(false);
                    audioButtonsStatus.SoundOff.gameObject.SetActive(true);
                }

                if (audioButtonsStatus.MusicIsOn)
                {
                    GameAudio.Instance.Music.Mixer.audioMixer.SetFloat("MusicVolume", 0);

                    audioButtonsStatus.MusicOff.gameObject.SetActive(false);
                    audioButtonsStatus.MusicOn.gameObject.SetActive(true);
                }
                else
                {
                    GameAudio.Instance.Music.Mixer.audioMixer.SetFloat("MusicVolume", -80);

                    audioButtonsStatus.MusicOn.gameObject.SetActive(false);
                    audioButtonsStatus.MusicOff.gameObject.SetActive(true);
                }
            }
        }
    }
}