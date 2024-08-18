using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class AnimalsStoreSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class SelectedEventAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalSnappedEvent))]
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
            [Inc] public readonly EcsPool<InGamePurchaseAnimals> Animals;

            [Opt] public readonly EcsTagPool<SnappedMarker> Snapped;
            [Opt] public readonly EcsTagPool<ScrollStartedMarker> ScrollStarted;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out SelectedEventAspect aspect))
            {
                Debug.Log("13");
                foreach (int animalShopWindow in _world.Where(out AnimalsShopWindowAspect animalShopWindowAspect))
                {
                    ref readonly InGamePurchaseAnimals animals =
                        ref animalShopWindowAspect.Animals.Read(animalShopWindow);

                    animalShopWindowAspect.ScrollStarted.TryDel(animals.Value[aspect.Indices.Read(entity).Value]);
                    animalShopWindowAspect.Snapped.TryAdd(animals.Value[aspect.Indices.Read(entity).Value]);
                }
            }

            foreach (int entity in _world.Where(out DeselectedEventAspect aspect))
            {
                foreach (int animalShopWindow in _world.Where(out AnimalsShopWindowAspect animalShopWindowAspect))
                {
                    ref readonly InGamePurchaseAnimals animals =
                        ref animalShopWindowAspect.Animals.Read(animalShopWindow);

                    Debug.Log(aspect.Indices.Read(entity).Value);
                    
                    animalShopWindowAspect.ScrollStarted.TryAdd(animals.Value[aspect.Indices.Read(entity).Value]);
                    animalShopWindowAspect.Snapped.TryDel(animals.Value[aspect.Indices.Read(entity).Value]);
                }
            }
        }
    }
}