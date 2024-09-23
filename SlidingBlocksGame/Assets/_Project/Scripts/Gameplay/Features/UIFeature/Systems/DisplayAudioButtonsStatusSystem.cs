using _Project.Scripts.Gameplay.Features.AudioFeature;
using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class UpdateGameAudioSystem : IEcsRun
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