using System;
using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.AudioFeature.Systems;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Utils
{
    public static class AudioUtils
    {
        private class AudioAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsTagPool<PlayAudioRequest> PlayAudio;
            [Opt] public readonly EcsTagPool<DeleteEntityCommand> DeleteEntity;
            [Opt] public readonly EcsPool<AudioSourceRef> AudioSource;
            [Opt] public readonly EcsTagPool<AudioLoopMarker> AudioLoopMarker;
            [Opt] public readonly EcsTagPool<RefreshCooldownRequest> Refresh;
            [Opt] public readonly EcsPool<AudioTypeRef> AudioTypes;
        }

        public static int Create(ScriptableEntityTemplate audioCfg)
        {
            EcsWorld world = EcsDefaultWorldSingletonProvider.Instance.Get();

            AudioAspect audioAspect = world.GetAspect<AudioAspect>();

            int audio = world.NewEntity(audioCfg);

            audioAspect.PlayAudio.Add(audio);

            ref AudioSourceRef audioSource = ref audioAspect.AudioSource.Add(audio);

            ref readonly AudioTypeRef audioType = ref audioAspect.AudioTypes.Read(audio);

            Debug.Log(audioType.Value);
            
            audioSource.Value = audioType.Value switch
            {
                AudioTypeRef.Type.NONE => GameAudio.Instance.SfxSource.Normal,
                AudioTypeRef.Type.SFX_NORMAL => GameAudio.Instance.SfxSource.Normal,
                AudioTypeRef.Type.SFX_SPECIAL => GameAudio.Instance.SfxSource.Special,
                AudioTypeRef.Type.MUSIC => GameAudio.Instance.MusicSource,
                _ => GameAudio.Instance.SfxSource.Normal
            };

            audioSource.Value.loop = audioAspect.AudioLoopMarker.Has(audio);

            audioAspect.DeleteEntity.Add(audio);

            return audio;
        }
    }
}