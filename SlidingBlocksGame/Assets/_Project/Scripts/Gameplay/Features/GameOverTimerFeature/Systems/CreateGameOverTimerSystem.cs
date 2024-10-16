using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameOverTimerFeature.Systems
{
    public class CreateGameOverTimerSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class GameAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CreateGameOverTimerRequest))]
            [Inc] public readonly EcsPool<GameOverTimerConfig> GameOverTimerConfig;
        }

        private class TimerAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsTagPool<RefreshCooldownRequest> Refresh;
            [Opt] public readonly EcsTagPool<LevelLifeTimeMarker> LevelLifeTime;
            [Opt] public readonly EcsTagPool<GameOverTimerCreatedEvent> GameOverTimerCreatedEvent;
        }

        public void Run()
        {
            foreach (int game in _world.Where(out GameAspect gameAspect))
            {
                TimerAspect timerAspect = _world.GetAspect<TimerAspect>();

                ref readonly GameOverTimerConfig timerConfig = ref gameAspect.GameOverTimerConfig.Read(game);
                
                entlong timer = _world.NewEntityLong(timerConfig.Value);

                timerAspect.Refresh.Add(timer.ID);
                timerAspect.LevelLifeTime.Add(timer.ID);
                timerAspect.GameOverTimerCreatedEvent.Add(timer.ID);
            }
        }
    }
}