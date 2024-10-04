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
            [Inc] public readonly EcsPool<SettingsPopup> SettingsPopups;
        }

        public void Run()
        {
            foreach (int popup in _world.Where(out SettingsPopupAspect settingsPopupAspect))
            {
                ref SettingsPopup settingsPopup = ref settingsPopupAspect.SettingsPopups.Get(popup);
                
                ref AudioButtonsStatus audioButtonsStatus = ref settingsPopupAspect.AudioButtonsStatus.Get(popup);

                if (audioButtonsStatus.SoundIsOn)
                {
                    GameAudio.Instance.Sfx.Mixer.audioMixer.SetFloat("SFXVolume", 0);

                    settingsPopup.SoundOff.gameObject.SetActive(false);
                    settingsPopup.SoundOn.gameObject.SetActive(true);
                }
                else
                {
                    GameAudio.Instance.Sfx.Mixer.audioMixer.SetFloat("SFXVolume", -80);

                    settingsPopup.SoundOn.gameObject.SetActive(false);
                    settingsPopup.SoundOff.gameObject.SetActive(true);
                }

                if (audioButtonsStatus.MusicIsOn)
                {
                    GameAudio.Instance.Music.Mixer.audioMixer.SetFloat("MusicVolume", 0);

                    settingsPopup.MusicOff.gameObject.SetActive(false);
                    settingsPopup.MusicOn.gameObject.SetActive(true);
                }
                else
                {
                    GameAudio.Instance.Music.Mixer.audioMixer.SetFloat("MusicVolume", -80);

                    settingsPopup.MusicOn.gameObject.SetActive(false);
                    settingsPopup.MusicOff.gameObject.SetActive(true);
                }
            }
        }
    }
}