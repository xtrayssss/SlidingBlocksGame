using _Project.Scripts.Gameplay.Features.PurchaseFeature.Components;
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Systems
{
    public class DisplayPurchasePriceSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PurchaseTag))]
            [IncImplicit(typeof(ScrollSnappedEvent))]
            [Inc] public readonly EcsPool<PurchaseWidget> PurchaseWidgets;

            [Inc] public readonly EcsPool<Purchase> Purchases;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref PurchaseWidget widget = ref aspect.PurchaseWidgets.Get(entity);
                
                widget.PurchaseStatusWidget.PriceText.text = aspect.Purchases.Get(entity).Price.ToString();
            }
        }
    }

}