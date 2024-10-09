using _Project.Scripts.Gameplay.Features.AudioBaseFeature.Components;
using _Project.Scripts.Gameplay.Features.AudioBaseFeature.Systems;
using _Project.Scripts.Gameplay.Features.AudioFeature.Systems;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.DestructionFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.IntegrationFeatures.AudioFeature.Systems;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;
using UnityEngine.UI;

namespace _Project.Scripts.Gameplay.Features.AudioBaseFeature
{
    public class AudioFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
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