using _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.UIFeature.Systems
{
    public class AnimalShopWindowSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class AnimalsShopWindowAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalsShopWindowTag))]
            [Inc] public readonly EcsPool<AnimalsShopWindow> AnimalsShopWindows;
        }

        private class PurchaseUpdatedStatusAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PurchaseTag))]
            [IncImplicit(typeof(PurchaseStatusUpdatedEvent))]
            [Inc] public readonly EcsPool<PurchaseWidget> PurchaseWidgets;
        }

        public void Run()
        {
            foreach (int purchase in _world.Where(out PurchaseUpdatedStatusAspect purchaseAspect))
            {
                foreach (int window in _world.Where(out AnimalsShopWindowAspect windowAspect))
                {
                    ref AnimalsShopWindow shopWindow =
                        ref windowAspect.AnimalsShopWindows.Get(window);

                    shopWindow.PurchaseStatusWidget.Current = purchaseAspect.PurchaseWidgets.Read(purchase).StatusWidget.Current;
                }
            }
        }
    }
}