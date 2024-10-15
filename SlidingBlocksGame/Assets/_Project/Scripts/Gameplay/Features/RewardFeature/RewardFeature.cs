using _Project.Scripts.Gameplay.Features.AudioFeature.Systems;
using _Project.Scripts.Gameplay.Features.RewardFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.AudioFeature.Systems;
using _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.UIFeature.Systems;
using _Project.Scripts.Gameplay.Features.RewardFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.RewardFeature
{
    public class RewardFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AutoDelTag<RewardEligibilityEvent>()
                .AddUnique(new RewardEligibilitySystem())
                .AddUnique(new RewardClaimSystem())
                //
                .AutoDelEntityTag<RewardUpdatedEvent>()
                .AddUnique(new RewardSystem())
                .AutoDelEntityComponent<UpdateRewardRequest>()
                
                // ui feature
                .AutoDelTag<RewardCoinDisplayCompletedEvent>()
                .AutoDelEntityTag<RewardCoinCountDisplayedEvent>()
                .AutoDelTag<ConfettiExplodedEvent>()
                .AddUnique(new RewardCatcherSystem())
                .AddUnique(new DisplayRewardSystem())
                
                // audio feature
                .AddAudioSystem<ConfettiExplodedEvent, ConfettiExplodedAudioConfig>()
                .AddAudioSystem<RewardCoinDisplayCompletedEvent, RewardCoinDisplayCompletedAudioConfig>()
                .AddUnique(new RewardCoinCountDisplayedAudioSystem());
        }
    }
}