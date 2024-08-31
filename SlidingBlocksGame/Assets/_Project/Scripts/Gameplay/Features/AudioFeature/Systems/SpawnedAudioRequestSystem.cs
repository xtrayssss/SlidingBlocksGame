using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Systems
{
    public class SpawnedAudioRequestSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private readonly AudioUtils _audioUtils;

        public SpawnedAudioRequestSystem(AudioUtils audioUtils) =>
            _audioUtils = audioUtils;

        private class SpawnedRetranslationAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(SpawnedEvent))]
            [Inc] public readonly EcsPool<TargetEntity> Targets;
        }

        private class SpawnedAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(SpawnedEvent))]
            [Inc] public readonly EcsPool<SpawnedAudioConfig> AudioConfigs;
        }

        private class TargetAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<SpawnedAudioConfig> AudioConfigs;
        }


        public void Run()
        {
            foreach (int entity in _world.Where(out SpawnedRetranslationAspect spawnedRetranslationAspect))
            {
                if (spawnedRetranslationAspect.Targets.Read(entity).Value.TryGetID(out int targetID))
                {
                    TargetAspect targetAspect = _world.GetAspect<TargetAspect>();

                    if (targetAspect.IsMatches(targetID))
                        _audioUtils.Create(targetAspect.AudioConfigs.Read(targetID).Value);
                }
            }

            foreach (int entity in _world.Where(out SpawnedAspect spawnedAspect))
                _audioUtils.Create(spawnedAspect.AudioConfigs.Read(entity).Value);
        }
    }
}