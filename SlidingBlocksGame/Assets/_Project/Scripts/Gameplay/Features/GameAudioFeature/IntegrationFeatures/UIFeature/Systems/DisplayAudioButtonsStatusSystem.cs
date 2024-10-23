using _Project.Scripts.Gameplay.Features.GameAudioFeature.Components;
using _Project.Scripts.Gameplay.Features.SettingsFeature.IntegrationFeatures.UIFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameAudioFeature.IntegrationFeatures.UIFeature.Systems
{
    public class DisplayAudioButtonsStatusSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class SettingsPopupAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(SettingsPopupTag))]
            [IncImplicit(typeof(AudioSettingsUpdatedEvent))]
            [Inc] public readonly EcsPool<AudioSettings> AudioButtonsStatus;

            [Inc] public readonly EcsPool<SettingsPopup> SettingsPopups;
        }

        public void Run()
        {
            foreach (int popup in _world.Where(out SettingsPopupAspect settingsPopupAspect))
            {
                ref SettingsPopup settingsPopup = ref settingsPopupAspect.SettingsPopups.Get(popup);

                ref AudioSettings audioSettings = ref settingsPopupAspect.AudioButtonsStatus.Get(popup);

                if (audioSettings.SoundIsOn)
                {
                    settingsPopup.SoundOff.gameObject.SetActive(false);
                    settingsPopup.SoundOn.gameObject.SetActive(true);
                }
                else
                {
                    settingsPopup.SoundOn.gameObject.SetActive(false);
                    settingsPopup.SoundOff.gameObject.SetActive(true);
                }

                if (audioSettings.MusicIsOn)
                {
                    settingsPopup.MusicOff.gameObject.SetActive(false);
                    settingsPopup.MusicOn.gameObject.SetActive(true);
                }
                else
                {
                    settingsPopup.MusicOn.gameObject.SetActive(false);
                    settingsPopup.MusicOff.gameObject.SetActive(true);
                }
            }
        }
    }
}