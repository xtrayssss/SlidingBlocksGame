using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class TileAudioRequestSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(TileGeneratedEvent))]
            [Inc] public readonly EcsPool<TargetEntity> Targets;
        }

        private class LevelAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<TileGeneratedAudioConfig> AudioConfigs;
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
                if (aspect.Targets.Read(entity).Value.TryGetID(out int level))
                {
                    LevelAspect levelAspect = _world.GetAspect<LevelAspect>();
                    
                    if (levelAspect.IsMatches(level))
                    {
                        int audio = _world.NewEntity(levelAspect.AudioConfigs.Read(level).Value);

                        AudioAspect audioAspect = _world.GetAspect<AudioAspect>();

                        audioAspect.PlayAudio.Add(audio);

                        audioAspect.AudioSources.Add(audio).Value = AudioSingleton.Instance.SfxSource;
                    }
                }
            }
        }
    }
}