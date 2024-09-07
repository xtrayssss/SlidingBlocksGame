using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Systems
{
    public class RewardCollectedAudioSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private readonly AudioUtils _audioUtils;

        public RewardCollectedAudioSystem(AudioUtils audioUtils) =>
            _audioUtils = audioUtils;

        private class RewardCollectedAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(RewardCollectedEvent))]
            [Inc] public readonly EcsPool<RewardCollectedAudioConfig> AudioConfigs;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out RewardCollectedAspect aspect))
            {
                Debug.Log("Audio");

                _audioUtils.Create(aspect.AudioConfigs.Read(entity).Value);
            }
        }
    }
}