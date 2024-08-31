using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Systems
{
    public class DeathAudioRequestSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private readonly AudioUtils _audioUtils;

        public DeathAudioRequestSystem(AudioUtils audioUtils) =>
            _audioUtils = audioUtils;
        
        private class DeathAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(DeathEvent))]
            [Inc] public readonly EcsPool<DeathAudioConfig> AudioConfigs;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out DeathAspect aspect))
                _audioUtils.Create(aspect.AudioConfigs.Read(entity).Value);
        }
    }
}