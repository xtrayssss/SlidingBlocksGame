using _Project.Scripts.Gameplay.Features.GameAudioFeature.Components;
using _Project.Scripts.Gameplay.Features.GameAudioFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.ButtonFeature.Components;
using DCFApixels.DragonECS;
using AudioSettings = _Project.Scripts.Gameplay.Features.GameAudioFeature.Components.AudioSettings;

namespace _Project.Scripts.Gameplay.Features.GameAudioFeature.Systems
{
    public class UpdateAudioSettingsSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class SoundButtonClickedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<SoundButtonTag> SoundButtonTag;
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> ButtonClickedEvent;
        }

        private class MusicButtonClickedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<MusicButtonTag> MusicButtonTag;
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> ButtonClickedEvent;
        }

        private class SettingsAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<AudioSettings> AudioSettings;
            [Opt] public readonly EcsTagPool<AudioSettingsUpdatedEvent> AudioSettingsUpdatedEvent;
        }

        private class UpdateAudioSettingsRequestAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<UpdateAudioSettingsRequest> UpdateAudioSettingsRequest;
        }

        public void Run()
        {
            foreach (int request in _world.Where(out UpdateAudioSettingsRequestAspect requestAspect))
            {
                foreach (int settings in _world.Where(out SettingsAspect settingsAspect))
                {
                    ref AudioSettings audioButtonsStatus = ref settingsAspect.AudioSettings.Get(settings);

                    ref readonly UpdateAudioSettingsRequest updateGameAudioRequest =
                        ref requestAspect.UpdateAudioSettingsRequest.Read(request);

                    audioButtonsStatus.MusicIsOn = updateGameAudioRequest.IsMusicOn;
                    audioButtonsStatus.SoundIsOn = updateGameAudioRequest.IsSoundOn;
                    
                    settingsAspect.AudioSettingsUpdatedEvent.Add(settings);
                    
                    _world.DelEntity(request);
                }
            }

            foreach (int _ in _world.Where(out SoundButtonClickedAspect _))
            {
                foreach (int settings in _world.Where(out SettingsAspect settingsPopupAspect))
                {
                    ref AudioSettings audioSettings = ref settingsPopupAspect.AudioSettings.Get(settings);
                    audioSettings.SoundIsOn = !audioSettings.SoundIsOn;

                    settingsPopupAspect.AudioSettingsUpdatedEvent.Add(settings);
                }
            }

            foreach (int _ in _world.Where(out MusicButtonClickedAspect _))
            {
                foreach (int settings in _world.Where(out SettingsAspect settingsPopupAspect))
                {
                    ref AudioSettings audioSettings = ref settingsPopupAspect.AudioSettings.Get(settings);

                    audioSettings.MusicIsOn = !audioSettings.MusicIsOn;

                    settingsPopupAspect.AudioSettingsUpdatedEvent.Add(settings);
                }
            }
        }
    }
}