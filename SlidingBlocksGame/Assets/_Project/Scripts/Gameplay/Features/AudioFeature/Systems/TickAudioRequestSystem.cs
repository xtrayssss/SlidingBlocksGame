using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Systems
{
    public class TickAudioRequestSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private readonly AudioUtils _audioUtils;

        public TickAudioRequestSystem(AudioUtils audioUtils) =>
            _audioUtils = audioUtils;
        
        private class TickAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(TickEvent))]
            [Inc] public readonly EcsPool<TickAudioConfig> AudioConfigs;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out TickAspect aspect))
                _audioUtils.Create(aspect.AudioConfigs.Read(entity).Value);
        }
    }
}