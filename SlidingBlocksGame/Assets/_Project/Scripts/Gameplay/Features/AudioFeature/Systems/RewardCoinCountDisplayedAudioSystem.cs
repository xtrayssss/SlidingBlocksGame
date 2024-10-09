using _Project.Scripts.Gameplay.Features.AudioBaseFeature;
using _Project.Scripts.Gameplay.Features.AudioBaseFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.UIFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Systems
{
    public class RewardCoinCountDisplayedAudioSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class RewardCoinCountDisplayedAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(RewardCoinCountDisplayedEvent))]
            [Inc] public readonly EcsPool<TargetEntity> TargetEntity;
        }

        private class TargetEntityAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<RewardCoinCountDisplayedAudioConfig> AudioConfigs;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out RewardCoinCountDisplayedAspect displayedAspect))
            {
                if (displayedAspect.TargetEntity.Read(entity).Value.TryGetID(out int targetID))
                {
                    TargetEntityAspect targetEntityAspect = _world.GetAspect<TargetEntityAspect>();

                    ref readonly RewardCoinCountDisplayedAudioConfig audioConfig =
                        ref targetEntityAspect.AudioConfigs.Read(targetID);

                    _world.NewAudioEntity(audioConfig.Value);
                }
            }
        }
    }
}