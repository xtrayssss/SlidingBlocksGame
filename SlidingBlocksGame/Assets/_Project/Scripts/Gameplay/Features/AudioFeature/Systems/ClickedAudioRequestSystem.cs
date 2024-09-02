using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Systems
{
    public class ClickedAudioRequestSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private readonly AudioUtils _audioUtils;

        public ClickedAudioRequestSystem(AudioUtils audioUtils) => 
            _audioUtils = audioUtils;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ButtonClickedEvent))]
            [Inc] public readonly EcsPool<ClickedAudioConfig> AudioConfigs;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                Debug.Log("Audio");
                _audioUtils.Create(aspect.AudioConfigs.Read(entity).Value);
            }
        }
    }
}