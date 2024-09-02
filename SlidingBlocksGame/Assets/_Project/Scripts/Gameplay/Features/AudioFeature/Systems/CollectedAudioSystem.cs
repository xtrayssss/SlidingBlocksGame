using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.CollectFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Systems
{
    public class CollectedAudioSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private readonly AudioUtils _audioUtils;

        public CollectedAudioSystem(AudioUtils audioUtils) =>
            _audioUtils = audioUtils;

        private class CollectedEventAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CollectedEvent))]
            [Inc] public readonly EcsPool<TargetEntity> Targets;
        }

        private class TargetAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<CollectedAudioConfig> AudioConfigs;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out CollectedEventAspect collectedEventAspect))
            {
                if (collectedEventAspect.Targets.Read(entity).Value.TryGetID(out int targetID))
                {
                    TargetAspect targetAspect = _world.GetAspect<TargetAspect>();

                    _audioUtils.Create(targetAspect.AudioConfigs.Read(targetID).Value);
                }
            }
        }
    }
}