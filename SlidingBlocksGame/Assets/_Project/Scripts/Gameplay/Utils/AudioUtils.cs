using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.AudioFeature.Systems;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using DCFApixels.DragonECS;

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
        }

        public static int Create(ScriptableEntityTemplate audioCfg)
        {
            EcsWorld world = EcsDefaultWorldSingletonProvider.Instance.Get();

            AudioAspect audioAspect = world.GetAspect<AudioAspect>();

            int audio = world.NewEntity(audioCfg);

            audioAspect.PlayAudio.Add(audio);
            audioAspect.AudioSource.Add(audio).Value = GameAudio.Instance.SfxSource;

            audioAspect.DeleteEntity.Add(audio);

            return audio;
        }
    }
}