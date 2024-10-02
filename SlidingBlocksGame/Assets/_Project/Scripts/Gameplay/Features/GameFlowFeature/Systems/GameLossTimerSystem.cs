using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.Systems
{
    public class GameLossTimerSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class TimerAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsTagPool<RefreshCooldownRequest> Refresh;
            [Opt] public readonly EcsTagPool<LevelLifeTimeMarker> LevelLifeTime;
        }

        private class HUDAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(HUDTag))]
            [IncImplicit(typeof(CreateGameLossTimerRequest))]
            [Inc] public readonly EcsPool<GameLossTimerUIConnect> GameLossTimerConnect;
        }

        private class GameLossTimerAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsTagPool<GameLossTimerOpenedEvent> GameLossTimerOpenedEvent;
        }

        public void Run()
        {
            foreach (int hud in _world.Where(out HUDAspect hudAspect))
            {
                ref readonly GameLossTimerUIConnect uiConnect =
                    ref hudAspect.GameLossTimerConnect.Read(hud);

                entlong timer = _world.NewEntityLong();

                uiConnect.Value.Connect(timer, applyTemplates: true);

                foreach (MonoEntityTemplateBase template in uiConnect.Value.MonoTemplates)
                    template.Apply(_world.id, timer.ID);

                TimerAspect timerAspect = _world.GetAspect<TimerAspect>();

                timerAspect.Refresh.Add(timer.ID);

                timerAspect.LevelLifeTime.Add(timer.ID);

                uiConnect.Value.transform.localScale = Vector3.zero;

                Tween.Scale(
                        target: uiConnect.Value.transform,
                        endValue: Vector3.one * 1.2f,
                        duration: 0.2f,
                        ease: Ease.OutBack)
                    .OnComplete(
                        uiConnect.Value,
                        static connect =>
                        {
                            if (!connect.Entity.TryGetID(out int id))
                                return;

                            EcsWorld world = connect.Entity.World;

                            GameLossTimerAspect gameLossTimerAspect = world.GetAspect<GameLossTimerAspect>();

                            gameLossTimerAspect.GameLossTimerOpenedEvent.Add(id);
                        });

                uiConnect.Value.gameObject.SetActive(true);
            }
        }
    }
}