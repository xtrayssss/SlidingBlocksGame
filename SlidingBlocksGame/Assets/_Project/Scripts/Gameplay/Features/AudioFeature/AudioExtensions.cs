using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.AudioFeature.Systems;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AudioFeature
{
    public static class AudioExtensions
    {
        public static EcsPipeline.Builder AddAudioSystem<TEvent, TConfig>(this EcsPipeline.Builder source)
            where TEvent : struct, IEcsTagComponent where TConfig : struct, IEcsAudioConfig =>
            source.Add(new AudioSystem<TEvent, TConfig>());
    }
}