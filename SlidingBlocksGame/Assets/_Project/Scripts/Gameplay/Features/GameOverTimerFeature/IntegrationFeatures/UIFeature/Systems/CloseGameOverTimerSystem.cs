using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.Components;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.IntegrationFeatures.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameOverTimerFeature.IntegrationFeatures.UIFeature.Systems
{
    public class CloseGameOverTimerSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class TimerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameOverTimerTag))]
            [IncImplicit(typeof(CloseGameOverTimerRequest))]
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;

            [Opt] public readonly EcsTagPool<GameOverTimerClosedMarker> GameOverTimerClosedMarker;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out TimerAspect aspect))
            {
                GameObjectConnect goConnect = aspect.GameObjectConnects.Read(entity);

                Tween
                    .Scale(
                        target: goConnect.Connect.transform,
                        endValue: Vector3.zero,
                        duration: 0.2f,
                        ease: Ease.InBack)
                    .OnComplete(
                        target: goConnect.Connect,
                        onComplete: static connect =>
                        {
                            if (!connect.Entity.TryGetID(out int id))
                                return;

                            EcsWorld world = connect.World;

                            int catcher = world.NewEntity();
                            GameOverTimerCatcherAspect gameOverTimerCatcherAspect =
                                world.GetAspect<GameOverTimerCatcherAspect>();
                            gameOverTimerCatcherAspect.CommonCatcherAspect.TargetEntities.Add(catcher).Value =
                                connect.Entity;
                            gameOverTimerCatcherAspect.CatchGameOverClosed.Add(catcher);

                            TimerAspect timerAspect = world.GetAspect<TimerAspect>();
                            timerAspect.GameOverTimerClosedMarker.Add(id);
                            connect.gameObject.SetActive(false);
                        });
            }
        }
    }
}