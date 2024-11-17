using _Project.Scripts.Gameplay.Features.AdFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.AdFeature.Systems
{
    public class CatchAdEventsSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;
        
        private class EventAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CatchAdClosedEventRequest> CatchAdClosedEventRequest;
            [Opt] public readonly EcsTagPool<AdCompletedEvent> AdCompletedEvent;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out EventAspect eventAspect)) 
                eventAspect.AdCompletedEvent.Add(entity);
        }
    }
}