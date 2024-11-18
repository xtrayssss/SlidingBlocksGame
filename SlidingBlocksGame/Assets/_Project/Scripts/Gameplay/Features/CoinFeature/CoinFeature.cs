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
                .AddSystem(new CatchCoinSystem())
                .AddSystem(new CreateCoinSystem())
                .AutoDelTag<CreateCoinRequest>()
                // 
                .AutoDelTag<CoinCollectedEvent>()
                .AddSystem(new CollectCoinSystem())
                //
                .AutoDelEntityTag<CoinsUpdatedEvent>()
                .AddSystem(new UpdateCoinsSystem())
                // coin destruction
                .AddSystem(new CoinDestroySystem())
                .AddSystem(new CoinDestructionSystem())
                // coin audio
                .AddAudioSystem<CoinCollectedEvent, CoinCollectedAudioConfig>()

                // ui feature
                .AddSystem(new DisplayCoinsSystem());
        }
    }
}