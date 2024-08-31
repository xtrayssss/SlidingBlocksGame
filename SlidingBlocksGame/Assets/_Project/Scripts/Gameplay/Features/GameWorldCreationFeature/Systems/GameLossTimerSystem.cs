using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class GameLossTimerSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class LevelAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CreateGameLossTimerRequest))]
            [Inc] public readonly EcsPool<GameLossTimerCfg> TimerConfigs;

            [Inc] public readonly EcsPool<GameScreen> GameScreen;
        }

        private class TimerAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsTagPool<RefreshCooldownRequest> Refresh;
            [Opt] public readonly EcsTagPool<LevelLifeTimeMarker> LevelLifeTime;
        }

        private class HUDAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameLossTimerConnect> GameLossTimerConnect;
        }

        public void Run()
        {
            foreach (int level in _world.Where(out LevelAspect levelAspect))
            {
                foreach (int hud in _world.Where(out HUDAspect hudAspect))
                {
                    ref readonly GameLossTimerConnect connect =
                        ref hudAspect.GameLossTimerConnect.Read(hud);

                    entlong timer = _world.NewEntityLong(levelAspect.TimerConfigs.Read(level).Value);

                    connect.Value.Connect(timer, false);

                    foreach (MonoEntityTemplateBase template in connect.Value.MonoTemplates)
                        template.Apply(_world.id, timer.ID);

                    TimerAspect timerAspect = _world.GetAspect<TimerAspect>();

                    timerAspect.Refresh.Add(timer.ID);

                    timerAspect.LevelLifeTime.Add(timer.ID);

                    connect.Value.transform.localScale = Vector3.zero;

                    Sequence.Create()
                        .Chain(Tween.Scale(connect.Value.transform, Vector3.one * 1.2f, 0.2f, Ease.OutQuad)
                            .Chain(Tween.Scale(connect.Value.transform, Vector3.one * 1f, 0.1f, Ease.InQuad)));

                    connect.Value.gameObject.SetActive(true);
                }
            }
        }
    }
}