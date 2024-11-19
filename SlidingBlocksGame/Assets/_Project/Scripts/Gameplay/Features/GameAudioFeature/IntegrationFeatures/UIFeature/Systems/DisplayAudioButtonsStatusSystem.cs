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

                settingsPopup.SoundOff.gameObject.SetActive(!audioSettings.SoundIsOn);
                settingsPopup.SoundOn.gameObject.SetActive(audioSettings.SoundIsOn);

                settingsPopup.MusicOff.gameObject.SetActive(!audioSettings.MusicIsOn);
                settingsPopup.MusicOn.gameObject.SetActive(audioSettings.MusicIsOn);
            }
        }
    }
}