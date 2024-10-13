<<<<<<<< HEAD:SlidingBlocksGame/Assets/_Project/Scripts/Gameplay/Features/VisualFeature/UIFeature/Systems/DisplayPriceAnimalSystem.cs
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
========
using _Project.Scripts.Gameplay.Features.PurchaseFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.IntegrationFeatures.UIFeature.Components;
>>>>>>>> recovery-branch:SlidingBlocksGame/Assets/_Project/Scripts/Gameplay/Features/CommonFeature/IntegrationFeatures/UIFeature/Systems/DisplayPurchasePriceSystem.cs
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.PurchaseFeature.IntegrationFeatures.UIFeature.Systems
{
    public class DisplayPriceAnimalSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PurchaseAnimalTag))]
            [IncImplicit(typeof(ScrollSnappedEvent))]
            [Inc] public readonly EcsPool<TextMeshProUGUIRef> PriceTexts;

            [Inc] public readonly EcsPool<Purchase> Purchases;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref TextMeshProUGUIRef price = ref aspect.PriceTexts.Get(entity);

                price.Value.text = aspect.Purchases.Get(entity).Price.ToString();
            }
        }
    }
}