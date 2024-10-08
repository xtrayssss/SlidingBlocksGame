using _Project.Scripts.Gameplay.Features.AudioBaseFeature.Components;
using _Project.Scripts.Gameplay.Features.AudioBaseFeature.Systems;
using _Project.Scripts.Gameplay.Features.AudioFeature.Systems;
using _Project.Scripts.Gameplay.Features.DestructionFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.IntegrationFeatures.AudioFeature.Systems;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AudioBaseFeature
{
    public class AudioFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AddAudioSystem<ButtonClickedEvent, ClickedAudioConfig>()
                .AddAudioSystem<DeathEvent, DeathAudioConfig>()
                .AddAudioSystem<TickEvent, TickAudioConfig>()
                .AddUnique(new GameFieldAudioSystem())
                .AddAudioSystem<CoinCollectedEvent, CollectedAudioConfig>()
                .AddAudioSystem<ConfettiExplodedEvent, ConfettiExplodedAudioConfig>()
                .AddAudioSystem<RewardCoinDisplayCompletedEvent, RewardCoinDisplayCompletedAudioConfig>()
                .AddUnique(new RewardCoinCountDisplayedAudioSystem())
                .AddAudioSystem<PurchasedEvent, PurchasedAudioConfig>()
                //
                .AddUnique(new AddAudioSourceSystem())
                .AddUnique(new PlaybackAudioSystem())
                .AddUnique(new AudioSystem())
                .AutoDelEntityTag<ApplyAudioEffectRequest>()
                .AutoDelEntityTag<PlayAudioRequest>()
                .AutoDelEntityTag<PlayOneShotAudioRequest>()
                .AutoDelEntityTag<StopAudioRequest>()
                .AutoDelEntityTag<RestartAudioRequest>();
        }
    }
}