using _Project.Scripts.Gameplay.Features.AudioBaseFeature.Components;
using _Project.Scripts.Gameplay.Features.AudioBaseFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

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