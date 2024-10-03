using _Project.Scripts.Gameplay.Features.AudioBaseFeature;
using _Project.Scripts.Gameplay.Features.AudioBaseFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Systems
{
    public class CoinAddedToTextAudioSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class CoinAddedToTextAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CoinAddedToTextEvent))]
            [Inc] public readonly EcsPool<TargetEntity> Targets;
        }

        private class TargetAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<CoinAddedToTextAudioConfig> AudioConfigs;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out CoinAddedToTextAspect collectedEventAspect))
            {
                if (collectedEventAspect.Targets.Read(entity).Value.TryGetID(out int targetID))
                {
                    TargetAspect targetAspect = _world.GetAspect<TargetAspect>();

                    _world.NewAudioEntity(targetAspect.AudioConfigs.Read(targetID).Value);
                }
            }
        }
    }
}