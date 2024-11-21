using _Project.Scripts.Gameplay.Features.AudioFeature.Extensions;
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
    public class RewardFeature<TMask> : EcsModule<TMask> where TMask : EcsAspect, new()
    {
        protected override void Import(Builder builder)
        {
            builder
                .AutoDelTag<RewardEligibilityEvent>()
                .AddSystem(new RewardEligibilitySystem())
                .AutoDelTag<RewardCollectedEvent>()
                .AddSystem(new RewardClaimSystem())
                //
                .AutoDelEntityTag<RewardUpdatedEvent>()
                .AddSystem(new UpdateRewardSystem())
                .AutoDelEntityComponent<UpdateRewardRequest>()
                
                // ui feature
                .AutoDelTag<RewardCoinDisplayCompletedEvent>()
                .AutoDelEntityTag<RewardCoinCountDisplayedEvent>()
                .AutoDelTag<ConfettiExplodedEvent>()
                .AddSystem(new RewardCatcherSystem())
                .AddSystem(new DisplayRewardStateSystem())
                .AddSystem(new OpenRewardWindowSystem())
                .AddSystem(new CloseRewardWindowSystem())
                
                // audio feature
                .AddAudioSystem<ConfettiExplodedEvent, ConfettiExplodedAudioConfig>()
                .AddAudioSystem<RewardCoinDisplayCompletedEvent, RewardCoinDisplayCompletedAudioConfig>()
                .AddSystem(new RewardCoinCountDisplayedAudioSystem());
        }
    }
}