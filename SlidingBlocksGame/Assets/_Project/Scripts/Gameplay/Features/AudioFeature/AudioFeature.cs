using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.AudioFeature.Extensions;
using _Project.Scripts.Gameplay.Features.AudioFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AudioFeature
{
    public class AudioFeature<TMask> : EcsModule<TMask> where TMask : EcsAspect, new()
    {
        protected override void Import(Builder builder)
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