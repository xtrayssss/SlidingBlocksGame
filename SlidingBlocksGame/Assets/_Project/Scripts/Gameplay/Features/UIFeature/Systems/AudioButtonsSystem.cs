using _Project.Scripts.Gameplay.Features.AudioFeature.Systems;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class AudioButtonsSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class SoundButtonClickedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<SoundButtonTag> _;
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> _1;
        }

        private class MusicButtonClickedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<MusicButtonTag> _;
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> _1;
        }

        private class GameScreen : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<GameScreenTag> Obstacles;
            [Inc] public readonly EcsPool<AudioButtonsStatus> SoundButtonViews;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out SoundButtonClickedAspect _))
            {
                foreach (var screen in _world.Where(out GameScreen gameScreenAspect))
                {
                    ref AudioButtonsStatus audioButtonsStatus = ref gameScreenAspect.SoundButtonViews.Get(screen);

                    audioButtonsStatus.SoundIsOn = !audioButtonsStatus.SoundIsOn;
                    
                    if (audioButtonsStatus.SoundIsOn)
                    {
                        GameAudio.Instance.SfxSource.volume = 1;

                        audioButtonsStatus.SoundOff.gameObject.SetActive(false);
                        audioButtonsStatus.SoundOn.gameObject.SetActive(true);
                    }
                    else
                    {
                        GameAudio.Instance.SfxSource.volume = 0;
                        
                        audioButtonsStatus.SoundOn.gameObject.SetActive(false);
                        audioButtonsStatus.SoundOff.gameObject.SetActive(true);
                    }
                }
            }

            foreach (int _ in _world.Where(out MusicButtonClickedAspect _))
            {
                foreach (int screen in _world.Where(out GameScreen gameScreenAspect))
                {
                    ref AudioButtonsStatus audioButtonsStatus = ref gameScreenAspect.SoundButtonViews.Get(screen);

                    audioButtonsStatus.MusicIsOn = !audioButtonsStatus.MusicIsOn;

                    if (audioButtonsStatus.MusicIsOn)
                    {
                        GameAudio.Instance.MusicSource.volume = 1;

                        audioButtonsStatus.MusicOff.gameObject.SetActive(false);
                        audioButtonsStatus.MusicOn.gameObject.SetActive(true);
                    }
                    else
                    {
                        GameAudio.Instance.MusicSource.volume = 0;

                        audioButtonsStatus.MusicOn.gameObject.SetActive(false);
                        audioButtonsStatus.MusicOff.gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}