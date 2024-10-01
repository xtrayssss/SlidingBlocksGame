using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.TweenFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Systems
{
    public class AudioSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class VolumeAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ApplyAudioEffectRequest))]
            [IncImplicit(typeof(VolumeEffectTag))]
            [Inc] public readonly EcsPool<TweenStartDelay> TweenStartDelay;

            [Inc] public readonly EcsPool<TweenEndDelay> TweenEndDelay;
            [Inc] public readonly EcsPool<TweenDuration> TweenDuration;
            [Inc] public readonly EcsPool<TweenTargetFloatValue> TweenTargetFloatValue;
            [Inc] public readonly EcsPool<TweenEase> TweenEase;
            [Inc] public readonly EcsPool<AudioTypeRef> AudioTypes;

            [Opt] public readonly EcsPool<TweenStartFloatValue> TweenStartFloatValue;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out VolumeAspect aspect))
            {
                ref readonly AudioTypeRef audioType = ref aspect.AudioTypes.Read(entity);

                ref readonly TweenStartDelay tweenStartDelay = ref aspect.TweenStartDelay.Read(entity);
                ref readonly TweenEndDelay tweenEndDelay = ref aspect.TweenEndDelay.Read(entity);
                ref readonly TweenTargetFloatValue
                    tweenTargetFloatValue = ref aspect.TweenTargetFloatValue.Read(entity);
                ref readonly TweenEase tweenEase = ref aspect.TweenEase.Read(entity);
                ref readonly TweenDuration tweenDuration = ref aspect.TweenDuration.Read(entity);

                AudioSource audioSource = audioType.Value switch
                {
                    AudioTypeRef.Type.NONE => GameAudio.Instance.Sfx.Normal,
                    AudioTypeRef.Type.SFX_NORMAL => GameAudio.Instance.Sfx.Normal,
                    AudioTypeRef.Type.SFX_SPECIAL => GameAudio.Instance.Sfx.Special,
                    AudioTypeRef.Type.MUSIC => GameAudio.Instance.Music.Source,
                    _ => GameAudio.Instance.Sfx.Normal
                };

                bool startFromCurrent = !aspect.TweenStartFloatValue.Has(entity);

                Tween.AudioVolume(
                    target: audioSource,
                    settings: new TweenSettings<float>
                    {
                        startFromCurrent = startFromCurrent,

                        startValue = startFromCurrent
                            ? audioSource.volume
                            : aspect.TweenStartFloatValue.Read(entity).Value,

                        endValue = tweenTargetFloatValue.Value,

                        settings = new TweenSettings
                        {
                            startDelay = tweenStartDelay.Value,
                            endDelay = tweenEndDelay.Value,
                            duration = tweenDuration.Value,
                            ease = tweenEase.Ease,
                            customEase = tweenEase.Curve
                        }
                    });
            }
        }
    }

    public class AudioSystem<TEvent, TConfig> : IEcsRun where TEvent : struct, IEcsTagComponent
        where TConfig : struct, IEcsAudioConfig
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<TEvent> Events;
            [Inc] public readonly EcsPool<TConfig> AudioConfigs;

            [Opt] public readonly EcsPool<AudioTypeRef> AudioTypes;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                int audio = _world.NewAudioEntity(aspect.AudioConfigs.Read(entity).Value);

                Debug.Log("AUDIO: " + typeof(TEvent).Name + aspect.AudioTypes.Read(audio).Value);
            }
        }
    }
}