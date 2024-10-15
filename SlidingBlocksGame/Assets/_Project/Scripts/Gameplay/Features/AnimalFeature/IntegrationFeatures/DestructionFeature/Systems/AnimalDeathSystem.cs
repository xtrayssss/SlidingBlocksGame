using _Project.Scripts.Gameplay.Features.AnimalFeature.Components;
using _Project.Scripts.Gameplay.Features.DestructionFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.DestructionFeature.Systems
{
    public class AnimalDeathSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class AnimalAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalTag))]
            [IncImplicit(typeof(DestructibleStrategyCompletedEvent))]
            [ExcImplicit(typeof(DiedEvent))]
            [Opt] public readonly EcsTagPool<DestroyViewRequest> DestroyView;
        }

        private class ViewDestroyedAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalTag))]
            [IncImplicit(typeof(ViewDestroyedEvent))]
            [Exc] public readonly EcsTagPool<DiedEvent> DiedEvent;
        }

        public void Run()
        {
            foreach (int animal in _world.Where(out AnimalAspect animalAspect))
                animalAspect.DestroyView.Add(animal);

            foreach (int animal in _world.Where(out ViewDestroyedAspect animalAspect))
                animalAspect.DiedEvent.Add(animal);
        }
    }
}