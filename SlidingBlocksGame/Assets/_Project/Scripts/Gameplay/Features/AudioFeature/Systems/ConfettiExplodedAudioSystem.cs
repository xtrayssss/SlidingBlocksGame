using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Systems
{
    public class ConfettiExplodedAudioSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private readonly AudioUtils _audioUtils;

        public ConfettiExplodedAudioSystem(AudioUtils audioUtils) =>
            _audioUtils = audioUtils;

        private class ConfettiExplodedAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ConfettiExplodedEvent))]
            [Inc] public readonly EcsPool<ConfettiExplodedAudioConfig> AudioConfigs;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out ConfettiExplodedAspect aspect))
            {
                Debug.Log("Audio");

                _audioUtils.Create(aspect.AudioConfigs.Read(entity).Value);
            }
        }
    }
}