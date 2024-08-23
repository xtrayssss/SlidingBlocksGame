using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;

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
            [Inc] public readonly EcsPool<AudioButtonsViews> SoundButtonViews;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out SoundButtonClickedAspect _))
            {
                foreach (var screen in _world.Where(out GameScreen gameScreenAspect))
                {
                    ref AudioButtonsViews audioButtonsViews = ref gameScreenAspect.SoundButtonViews.Get(screen);

                    audioButtonsViews.SoundIsOn = !audioButtonsViews.SoundIsOn;
                    
                    if (audioButtonsViews.SoundIsOn)
                    {
                        audioButtonsViews.SoundOff.gameObject.SetActive(false);
                        audioButtonsViews.SoundOn.gameObject.SetActive(true);
                    }
                    else
                    {
                        audioButtonsViews.SoundOn.gameObject.SetActive(false);
                        audioButtonsViews.SoundOff.gameObject.SetActive(true);
                    }
                }
            }

            foreach (int _ in _world.Where(out MusicButtonClickedAspect _))
            {
                foreach (int screen in _world.Where(out GameScreen gameScreenAspect))
                {
                    ref AudioButtonsViews audioButtonsViews = ref gameScreenAspect.SoundButtonViews.Get(screen);

                    audioButtonsViews.MusicIsOn = !audioButtonsViews.MusicIsOn;

                    if (audioButtonsViews.MusicIsOn)
                    {
                        audioButtonsViews.MusicOff.gameObject.SetActive(false);
                        audioButtonsViews.MusicOn.gameObject.SetActive(true);
                    }
                    else
                    {
                        audioButtonsViews.MusicOn.gameObject.SetActive(false);
                        audioButtonsViews.MusicOff.gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}