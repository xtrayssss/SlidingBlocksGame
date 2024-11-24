using _Project.Scripts.Gameplay.Features.AudioFeature.Extensions;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.IntegrationFeatures.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.IntegrationFeatures.UIFeature.Systems;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.PurchaseFeature
{
    public class PurchaseFeature<TMask> : EcsModule<TMask> where TMask : EcsAspect, new()
    {
        protected override void Import(Builder builder)
        {
            builder
                .AutoDelTag<PurchasedEvent>()
                .AddSystem(new PurchaseSystem())
                //
                .AddSystem(new RotatePurchaseSystem())
                
                // ui feature
                .AddSystem(new DisplayPurchasePriceSystem())
                .AutoDelTag<PurchaseStatusUpdatedEvent>()
                .AddSystem(new DisplayPurchaseStatusSystem())
                // audio feature
                .AddAudioSystem<PurchasedEvent, PurchasedAudioConfig>();
        }
    }
}