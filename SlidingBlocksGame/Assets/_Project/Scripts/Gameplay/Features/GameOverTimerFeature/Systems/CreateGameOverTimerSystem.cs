using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameOverTimerFeature.Systems
{
    public class CreateGameOverTimerSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class CreateRequestAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CreateGameOverTimerRequest> CreateGameOverTimerRequest;
        }

        private class TimerAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsTagPool<RefreshCooldownRequest> Refresh;
            [Opt] public readonly EcsTagPool<LevelLifeTimeMarker> LevelLifeTime;
            [Opt] public readonly EcsTagPool<GameOverTimerCreatedEvent> GameOverTimerCreatedEvent;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out CreateRequestAspect _))
            {
                entlong timer = _world.NewEntityLong();

                TimerAspect timerAspect = _world.GetAspect<TimerAspect>();

                timerAspect.Refresh.Add(timer.ID);
                timerAspect.LevelLifeTime.Add(timer.ID);
                timerAspect.GameOverTimerCreatedEvent.Add(timer.ID);
            }
        }
    }
}