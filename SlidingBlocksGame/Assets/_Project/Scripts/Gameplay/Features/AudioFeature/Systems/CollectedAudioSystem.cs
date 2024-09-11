using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.CollectFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;
using UnityEngine;

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
            [Inc] public readonly EcsPool<CollectedTargetEntity> CollectedTarget;
        }

        private class TargetCollectedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<CollectedAudioConfig> AudioConfigs;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out CollectedEventAspect collectedEventAspect))
            {
                if (!collectedEventAspect.CollectedTarget.Read(entity).Value.TryGetID(out int targetID)) 
                    continue;
                
                TargetCollectedAspect targetCollectedAspect = _world.GetAspect<TargetCollectedAspect>();

                _audioUtils.Create(targetCollectedAspect.AudioConfigs.Read(targetID).Value);

                Debug.Log("Collected Audio");
            }
        }
    }
}