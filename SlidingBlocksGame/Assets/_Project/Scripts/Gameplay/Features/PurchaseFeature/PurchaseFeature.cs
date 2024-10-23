using _Project.Scripts.Gameplay.Features.AudioFeature.Extensions;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.IntegrationFeatures.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.IntegrationFeatures.UIFeature.Systems;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.Systems;
using _Project.Scripts.Infrastructure;
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
                //
                .AddUnique(new RotatePurchaseSystem())
                
                // ui feature
                .AddUnique(new DisplayPurchasePriceSystem())
                .AddUnique(new DisplayPurchaseStatusSystem())
                // audio feature
                .AddAudioSystem<PurchasedEvent, PurchasedAudioConfig>();
        }
    }
}