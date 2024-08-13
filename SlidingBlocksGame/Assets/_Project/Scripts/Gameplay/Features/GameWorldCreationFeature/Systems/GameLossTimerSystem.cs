using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class GameLossTimerSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalPositionedEvent))]
            [Inc] public readonly EcsPool<GameLossTimerCfg> TimerConfigs;

            [Inc] public readonly EcsPool<GameScreen> GameScreen;
        }

        private class TimerAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsTagPool<RefreshCooldownRequest> Refresh;
            [Opt] public readonly EcsTagPool<LevelLifeTimeMarker> LevelLifeTime;
        }

        private class GameScreenAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameLossTimerConnect> GameLossTimerConnect;
        }

        public void Run()
        {
            foreach (int level in _world.Where(out Aspect aspect))
            {
                GameScreenAspect gameScreenAspect = _world.GetAspect<GameScreenAspect>();

                if (aspect.GameScreen.Read(level).Value.TryGetID(out int gameScreenID))
                {
                    ref readonly GameLossTimerConnect connect =
                        ref gameScreenAspect.GameLossTimerConnect.Read(gameScreenID);

                    entlong timer = _world.NewEntityLong(aspect.TimerConfigs.Read(level).Value);

                    connect.Value.Connect(timer, false);

                    foreach (MonoEntityTemplateBase template in connect.Value.MonoTemplates)
                        template.Apply(_world.id, timer.ID);

                    TimerAspect timerAspect = _world.GetAspect<TimerAspect>();

                    timerAspect.Refresh.Add(timer.ID);

                    timerAspect.LevelLifeTime.Add(timer.ID);

                    connect.Value.gameObject.SetActive(true);
                }
            }
        }
    }
}