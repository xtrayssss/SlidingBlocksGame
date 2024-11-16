using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.ButtonFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.ButtonFeature.Systems
{
    public class CatchButtonEventsSystem : IEcsRun
    {
        [EcsInject] private readonly EcsWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [Inc] public EcsTagPool<CatchButtonEventRequest> CatchButtonEventRequest;
            [Opt] public EcsTagPool<ButtonClickedEvent> ButtonClickedEvent;
        }
        
        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect)) 
                aspect.ButtonClickedEvent.Add(entity);
        }
    }
}