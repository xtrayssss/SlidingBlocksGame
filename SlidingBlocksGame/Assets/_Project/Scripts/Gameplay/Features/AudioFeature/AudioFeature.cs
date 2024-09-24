using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.AudioFeature.Systems;
using _Project.Scripts.Gameplay.Features.CollectFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AudioFeature
{
    public class AudioFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AddAudioSystem<AnimalSpawnedEvent, AnimalSpawnedAudioConfig>()
                .AddAudioSystem<ButtonClickedEvent, ClickedAudioConfig>()
                .AddAudioSystem<DeathEvent, DeathAudioConfig>()
                .AddAudioSystem<TickEvent, TickAudioConfig>()
                .AddUnique(new GameFieldAudioSystem())
                .AddAudioSystem<CoinCollectedEvent, CollectedAudioConfig>()
                .AddAudioSystem<ConfettiExplodedEvent, ConfettiExplodedAudioConfig>()
                .AddAudioSystem<RewardCollectedEvent, RewardCollectedAudioConfig>()
                .AddUnique(new CoinAddedToTextAudioSystem())
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