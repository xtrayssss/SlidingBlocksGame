using _Project.Scripts.Gameplay.Features.AdFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AdFeature.Systems
{
    public class CatchAdEventsSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;
        
        private class EventAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<AdCompletedEvent> AdCompletedEvent;
            [Exc] public readonly EcsTagPool<AdCompletedProcessedMarker> AdCompletedProcessedMarker;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out EventAspect eventAspect)) 
                eventAspect.AdCompletedProcessedMarker.Add(entity);
        }
    }
}