using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class ButtonPlayAudioRequestSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ButtonClickedEvent))]
            [Inc] public readonly EcsPool<AudioConfig> AudioConfigs;
        }
        private class AudioAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsTagPool<PlayAudioRequest> PlayAudio;
            [Opt] public readonly EcsPool<AudioSourceRef> AudioSources;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                int audio = _world.NewEntity(aspect.AudioConfigs.Read(entity).Value);

                AudioAspect audioAspect = _world.GetAspect<AudioAspect>();
                
                audioAspect.PlayAudio.Add(audio);

                audioAspect.AudioSources.Add(audio).Value = AudioSingleton.Instance.SfxSource;
            }
        }
    }
}