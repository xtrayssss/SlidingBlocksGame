using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.IntegrationFeatures.UIFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameOverTimerFeature.IntegrationFeatures.UIFeature.Systems
{
    public class CatchGameOverTimerSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        public void Run()
        {
            foreach (int entity in _world.Where(out GameOverTimerCatcherAspect catcherAspect))
            {
                if (catcherAspect.CommonCatcherAspect.TargetEntities.Read(entity).Value.TryGetID(out int timerID))
                    catcherAspect.GameOverTimerClosedEvent.Add(timerID);
                
                _world.DelEntity(entity);
            }
        }
    }
}