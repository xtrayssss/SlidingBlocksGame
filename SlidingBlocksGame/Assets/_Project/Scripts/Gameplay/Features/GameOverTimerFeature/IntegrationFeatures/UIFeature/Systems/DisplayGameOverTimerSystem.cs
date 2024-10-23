using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.Components;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.GameScreenFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Extensions;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameOverTimerFeature.IntegrationFeatures.UIFeature.Systems
{
    public class DisplayGameOverTimerSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class TimerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameOverTimerCreatedEvent))]
            [Opt] public readonly EcsTagPool<GameOverTimerOpenedEvent> GameOverTimerOpenedEvent;
        }

        private class GameScreenAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameScreen> GameScreens;
        }

        public void Run()
        {
            foreach (int timerID in _world.Where(out TimerAspect _))
            {
                foreach (int screen in _world.Where(out GameScreenAspect gameScreenAspect))
                {
                    ref readonly GameScreen gameScreen = ref gameScreenAspect.GameScreens.Read(screen);

                    gameScreen.GameOverTimerWidgetConnect.ConnectUI(timerID.ToEntityLong(_world));

                    gameScreen.GameOverTimerWidgetConnect.transform.localScale = Vector3.zero;

                    Tween.Scale(
                            target: gameScreen.GameOverTimerWidgetConnect.transform,
                            endValue: Vector3.one * 1.2f,
                            duration: 0.2f,
                            ease: Ease.OutBack)
                        .OnComplete(
                            gameScreen.GameOverTimerWidgetConnect,
                            static connect =>
                            {
                                if (!connect.Entity.TryGetID(out int id))
                                    return;

                                EcsWorld world = connect.Entity.World;

                                TimerAspect timerAspect = world.GetAspect<TimerAspect>();

                                timerAspect.GameOverTimerOpenedEvent.Add(id);
                            });

                    gameScreen.GameOverTimerWidgetConnect.gameObject.SetActive(true);
                }
            }
        }
    }
}