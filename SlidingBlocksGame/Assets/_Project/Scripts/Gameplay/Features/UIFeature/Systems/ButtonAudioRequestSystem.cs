using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class ButtonAudioRequestSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ButtonClickedEvent))]
            [Inc] public readonly EcsPool<ClickedAudioConfig> AudioConfigs;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect _))
            {
                int request = _world.NewEntity();
                
                _world.GetPool<AudioRequest>().Add(request);
                _world.GetPool<AudioConfig>().Add(request).Value = _world.GetPool<ClickedAudioConfig>().Get(entity).Value;
                _world.GetPool<DeleteEntityCommand>().Add(request);
            }
        }
    }
}