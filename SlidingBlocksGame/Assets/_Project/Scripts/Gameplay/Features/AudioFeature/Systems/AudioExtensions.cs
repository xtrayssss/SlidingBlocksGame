using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Systems
{
    public static class AudioExtensions
    {
        public static EcsPipeline.Builder AddAudioSystem<TEvent, TConfig>(this EcsPipeline.Builder source)
            where TEvent : struct, IEcsTagComponent where TConfig : struct, IEcsAudioConfig =>
            source.Add(new AudioSystem<TEvent, TConfig>());

        private class AudioAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsTagPool<PlayOneShotAudioRequest> PlayOneShotAudioRequest;
            [Opt] public readonly EcsTagPool<PlayAudioRequest> PlayAudioRequest;
            [Opt] public readonly EcsPool<AudioSourceRef> AudioSource;
            [Opt] public readonly EcsTagPool<AudioLoopMarker> AudioLoopMarker;
            [Opt] public readonly EcsTagPool<RefreshCooldownRequest> Refresh;
            [Opt] public readonly EcsPool<AudioTypeRef> AudioTypes;
        }

        public static int NewAudioEntity(this EcsWorld source, ScriptableEntityTemplate audioCfg)
        {
            AudioAspect audioAspect = source.GetAspect<AudioAspect>();

            int audio = source.NewEntity(audioCfg);

            ref AudioSourceRef audioSource = ref audioAspect.AudioSource.Add(audio);

            ref readonly AudioTypeRef audioType = ref audioAspect.AudioTypes.Read(audio);

            if ((audioType.Value & AudioTypeRef.Type.SFX) != 0)
                audioAspect.PlayOneShotAudioRequest.Add(audio);
            else
                audioAspect.PlayAudioRequest.Add(audio);

            audioSource.Value = audioType.GetAudioSource();

            audioSource.Value.loop = audioAspect.AudioLoopMarker.Has(audio);

            return audio;
        }

        public static AudioSource GetAudioSource(this in AudioTypeRef source)
        {
            return source.Value switch
            {
                AudioTypeRef.Type.NONE => GameAudio.Instance.Sfx.Normal,
                AudioTypeRef.Type.SFX_NORMAL => GameAudio.Instance.Sfx.Normal,
                AudioTypeRef.Type.SFX_SPECIAL => GameAudio.Instance.Sfx.Special,
                AudioTypeRef.Type.MUSIC => GameAudio.Instance.Music.Source,
                _ => GameAudio.Instance.Sfx.Normal
            };
        }
    }
}