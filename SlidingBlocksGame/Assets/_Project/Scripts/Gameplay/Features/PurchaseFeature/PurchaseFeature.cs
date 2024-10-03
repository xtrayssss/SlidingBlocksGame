using _Project.Scripts.Gameplay.Features.PurchaseFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.Systems;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.PurchaseFeature
{
    public class PurchaseFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AutoDelTag<PurchasedEvent>()
                .AddUnique(new PurchaseSystem())
                .AutoDelEntityTag<PurchasesClearedEvent>()
                .AddUnique(new UpdatePurchaseSystem())
                .AutoDelEntityTag<ClearPurchasesRequest>()
                .AddUnique(new RotatePurchaseSystem());
        }
    }
}