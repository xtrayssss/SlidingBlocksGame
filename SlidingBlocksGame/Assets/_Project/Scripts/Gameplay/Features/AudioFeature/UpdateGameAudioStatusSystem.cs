using _Project.Scripts.Gameplay.Features.AudioBaseFeature.Components;
using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Systems;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AudioFeature
{
    public class UpdateGameAudioStatusSystem : IEcsRun
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

        private class UpdateGameAudioRequestAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<UpdateGameAudioRequest> UpdateGameAudioRequest;
            [Inc] public readonly EcsPool<TargetEntity> TargetEntity;
        }

        private class SettingsPopupAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(SettingsPopupTag))]
            [Inc] public readonly EcsPool<AudioButtonsStatus> AudioButtonsStatus;

            [Opt] public readonly EcsTagPool<GameAudioUpdatedEvent> GameAudioUpdatedEvent;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out SoundButtonClickedAspect _))
            {
                foreach (int popup in _world.Where(out SettingsPopupAspect settingsPopupAspect))
                {
                    ref AudioButtonsStatus audioButtonsStatus = ref settingsPopupAspect.AudioButtonsStatus.Get(popup);
                    audioButtonsStatus.SoundIsOn = !audioButtonsStatus.SoundIsOn;

                    settingsPopupAspect.GameAudioUpdatedEvent.Add(popup);
                }
            }

            foreach (int _ in _world.Where(out MusicButtonClickedAspect _))
            {
                foreach (int popup in _world.Where(out SettingsPopupAspect settingsPopupAspect))
                {
                    ref AudioButtonsStatus audioButtonsStatus = ref settingsPopupAspect.AudioButtonsStatus.Get(popup);

                    audioButtonsStatus.MusicIsOn = !audioButtonsStatus.MusicIsOn;

                    settingsPopupAspect.GameAudioUpdatedEvent.Add(popup);
                }
            }

            foreach (int request in _world.Where(out UpdateGameAudioRequestAspect updateGameAudioRequestAspect))
            {
                if (!updateGameAudioRequestAspect.TargetEntity.Read(request).Value.TryGetID(out int targetID))
                    continue;

                SettingsPopupAspect settingsPopupAspect = _world.GetAspect<SettingsPopupAspect>();

                ref AudioButtonsStatus audioButtonsStatus = ref settingsPopupAspect.AudioButtonsStatus.Get(targetID);

                ref readonly var updateGameAudioRequest =
                    ref updateGameAudioRequestAspect.UpdateGameAudioRequest.Read(request);

                audioButtonsStatus.MusicIsOn = updateGameAudioRequest.IsMusicOn;
                audioButtonsStatus.SoundIsOn = updateGameAudioRequest.IsSoundOn;

                settingsPopupAspect.GameAudioUpdatedEvent.Add(targetID);
            }
        }
    }
}