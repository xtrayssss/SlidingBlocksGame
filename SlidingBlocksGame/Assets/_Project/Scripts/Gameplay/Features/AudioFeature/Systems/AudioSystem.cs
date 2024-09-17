using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Systems
{
    public class AudioSystem<TEvent, TConfig> : IEcsRun where TEvent : struct, IEcsTagComponent
        where TConfig : struct, IEcsAudioConfig
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<TEvent> Events;
            [Inc] public readonly EcsPool<TConfig> AudioConfigs;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                Debug.Log("AUDIO: " + typeof(TEvent).Name);

                AudioUtils.Create(aspect.AudioConfigs.Read(entity).Value);
            }
        }
    }
}