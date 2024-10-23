using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.GameScreenFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameScreenFeature.Systems
{
    public class PlayWidgetSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class LevelChangedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<LevelChangedEvent> LevelChangedEvent;
            [Inc] public readonly EcsTagPool<LevelTag> LevelTag;
        }

        private class GameScreenAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameScreen> GameScreens;
        }

        private class PlayWidgetAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<PlayWidget> PlayWidgets;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out LevelChangedAspect _))
            {
                foreach (int screen in _world.Where(out GameScreenAspect gameScreenAspect))
                {
                    ref readonly GameScreen gameScreen = ref gameScreenAspect.GameScreens.Read(screen);

                    if (!gameScreen.PlayWidgetConnect.Entity.TryGetID(out int playWidgetID))
                        continue;

                    PlayWidgetAspect playWidgetAspect = _world.GetAspect<PlayWidgetAspect>();
                    ref PlayWidget playWidget = ref playWidgetAspect.PlayWidgets.Get(playWidgetID);
                    playWidget.PlayButton.gameObject.SetActive(false);
                    playWidget.ReplayButton.gameObject.SetActive(true);
                }
            }
        }
    }
}