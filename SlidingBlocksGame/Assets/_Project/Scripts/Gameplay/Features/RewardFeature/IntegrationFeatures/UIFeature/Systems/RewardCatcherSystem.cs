using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.UIFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.UIFeature.Systems
{
    public class RewardCatcherSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        public void Run()
        {
            foreach (int entity in _world.Where(out RewardCatcherAspect.ConfettiCatcher catcherAspect))
            {
                ref readonly TargetEntity targetEntity =
                    ref catcherAspect.CommonCatcherAspect.TargetEntities.Read(entity);

                if (targetEntity.Value.TryGetID(out int targetID))
                    catcherAspect.ConfettiExplodedEvent.Add(targetID);

                _world.DelEntity(entity);
            }

            foreach (int entity in _world.Where(out RewardCatcherAspect.CoinCountDisplayedCatcher catcherAspect))
            {
                CommonCatcherAspect commonCatcherAspect = catcherAspect.CommonCatcherAspect;

                ref readonly TargetEntity targetEntity =
                    ref commonCatcherAspect.TargetEntities.Read(entity);

                if (targetEntity.Value.TryGetID(out _))
                {
                    int @event = _world.NewEntity();
                    catcherAspect.RewardCoinCountDisplayedEvent.Add(@event);
                    commonCatcherAspect.TargetEntities.Add(@event).Value = targetEntity.Value;
                }

                _world.DelEntity(entity);
            }

            foreach (int entity in _world.Where(out RewardCatcherAspect.CoinDisplayCompletedCatcher catcherAspect))
            {
                CommonCatcherAspect commonCatcherAspect = catcherAspect.CommonCatcherAspect;

                ref readonly TargetEntity targetEntity =
                    ref commonCatcherAspect.TargetEntities.Read(entity);

                if (targetEntity.Value.TryGetID(out int targetID)) 
                    catcherAspect.RewardCoinDisplayCompletedEvent.Add(targetID);

                _world.DelEntity(entity);
            }
        }
    }
}