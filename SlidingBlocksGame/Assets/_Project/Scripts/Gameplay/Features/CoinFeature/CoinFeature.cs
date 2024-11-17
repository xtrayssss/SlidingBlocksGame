using _Project.Scripts.Gameplay.Features.AudioFeature.Extensions;
using _Project.Scripts.Gameplay.Features.CoinFeature.Components;
using _Project.Scripts.Gameplay.Features.CoinFeature.IntegrationFeatures.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.CoinFeature.IntegrationFeatures.DestructionFeature.Systems;
using _Project.Scripts.Gameplay.Features.CoinFeature.IntegrationFeatures.UIFeature.Systems;
using _Project.Scripts.Gameplay.Features.CoinFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CoinFeature
{
    public class CoinFeature<TMask> : EcsModule<TMask> where TMask : EcsAspect, new()
    {
        protected override void Import(Builder builder)
        {
            builder.AutoDelTag<CoinSpawnedEvent>()
                .AutoDelTag<CoinCollectAnimationCompletedEvent>()
                .AutoDelTag<CoinDestroyAnimationCompletedEvent>()
                .AddUnique(new CatchCoinSystem())
                .AddUnique(new CreateCoinSystem())
                .AutoDelTag<CreateCoinRequest>()
                // 
                .AutoDelTag<CoinCollectedEvent>()
                .AddUnique(new CollectCoinSystem())
                //
                .AutoDelEntityTag<CoinsUpdatedEvent>()
                .AddUnique(new UpdateCoinsSystem())
                // coin destruction
                .AddUnique(new CoinDestroySystem())
                .AddUnique(new CoinDestructionSystem())
                // coin audio
                .AddAudioSystem<CoinCollectedEvent, CoinCollectedAudioConfig>()

                // ui feature
                .AddUnique(new DisplayCoinsSystem());
        }
    }
}