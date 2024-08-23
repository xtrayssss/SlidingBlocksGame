using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class ScrollSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class SelectedEventAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ScrollSnappedEvent))]
            [Inc] public readonly EcsPool<AnimalSelectedIndex> Indices;
        }

        private class DeselectedEventAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ScrollStartedEvent))]
            [Inc] public readonly EcsPool<AnimalSelectedIndex> Indices;
        }

        private class AnimalsShopWindowAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalsShopWindowTag))]
            [Inc] public readonly EcsPool<AnimalPurchases> Animals;

            [Opt] public readonly EcsTagPool<ScrollSnappedMarker> SnappedMarker;
            [Opt] public readonly EcsTagPool<ScrollStartedMarker> StartedMarker;            
            
            [Opt] public readonly EcsTagPool<ScrollSnappedEvent> SnappedEvent;
            [Opt] public readonly EcsTagPool<ScrollStartedEvent> StartedEvent;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out SelectedEventAspect aspect))
            {
                foreach (int animalShopWindow in _world.Where(out AnimalsShopWindowAspect animalShopWindowAspect))
                {
                    ref readonly AnimalPurchases animals =
                        ref animalShopWindowAspect.Animals.Read(animalShopWindow);

                    animalShopWindowAspect.SnappedEvent.TryAdd(animals.Entities[aspect.Indices.Read(entity).Value]);
                    animalShopWindowAspect.StartedEvent.TryDel(animals.Entities[aspect.Indices.Read(entity).Value]);
                    
                    animalShopWindowAspect.SnappedMarker.TryAdd(animals.Entities[aspect.Indices.Read(entity).Value]);
                    animalShopWindowAspect.StartedMarker.TryDel(animals.Entities[aspect.Indices.Read(entity).Value]);
                }
            }

            foreach (int entity in _world.Where(out DeselectedEventAspect aspect))
            {
                foreach (int animalShopWindow in _world.Where(out AnimalsShopWindowAspect animalShopWindowAspect))
                {
                    ref readonly AnimalPurchases animals =
                        ref animalShopWindowAspect.Animals.Read(animalShopWindow);

                    animalShopWindowAspect.SnappedEvent.TryDel(animals.Entities[aspect.Indices.Read(entity).Value]);
                    animalShopWindowAspect.StartedEvent.TryAdd(animals.Entities[aspect.Indices.Read(entity).Value]);

                    animalShopWindowAspect.SnappedMarker.TryDel(animals.Entities[aspect.Indices.Read(entity).Value]);
                    animalShopWindowAspect.StartedMarker.TryAdd(animals.Entities[aspect.Indices.Read(entity).Value]);
                }
            }
        }
    }
}