using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.Systems
{
    public class CreateGameOverTimerSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class TimerAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsTagPool<RefreshCooldownRequest> Refresh;
            [Opt] public readonly EcsTagPool<LevelLifeTimeMarker> LevelLifeTime;
        }

        private class GameScreenAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameScreenTag))]
            [IncImplicit(typeof(CreateGameOverTimerRequest))]
            [Inc] public readonly EcsPool<GameScreen> GameScreens;
        }

        private class GameLossTimerAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsTagPool<GameOverTimerOpenedEvent> GameOverTimerOpenedEvent;
        }

        public void Run()
        {
            foreach (int screen in _world.Where(out GameScreenAspect gameScreenAspect))
            {
                ref readonly GameScreen gameScreen = ref gameScreenAspect.GameScreens.Read(screen);

                entlong timer = _world.NewEntityLong();

                gameScreen.GameOverTimerWidgetConnect.Connect(timer, applyTemplates: true);

                foreach (MonoEntityTemplateBase template in gameScreen.GameOverTimerWidgetConnect.MonoTemplates)
                    template.Apply(_world.id, timer.ID);

                TimerAspect timerAspect = _world.GetAspect<TimerAspect>();

                timerAspect.Refresh.Add(timer.ID);

                timerAspect.LevelLifeTime.Add(timer.ID);

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

                            GameLossTimerAspect gameLossTimerAspect = world.GetAspect<GameLossTimerAspect>();

                            gameLossTimerAspect.GameOverTimerOpenedEvent.Add(id);
                        });

                gameScreen.GameOverTimerWidgetConnect.gameObject.SetActive(true);
            }
        }
    }
}