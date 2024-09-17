using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Systems
{
    public class RewardSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class RequestAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<UpdateRewardRequest> UpdateRewardRequest;
            [Opt] public readonly EcsPool<TargetEntity> TargetEntities;
        }

        private class TargetEntityAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<RewardsCount> RewardsCounts;
            [Inc] public readonly EcsPool<RewardCollectedAt> RewardCollectedAt;

            [Opt] public readonly EcsTagPool<RewardUpdatedEvent> RewardUpdatedEvent;
            [Opt] public readonly EcsPool<TargetEntity> TargetEntity;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out RequestAspect aspect))
            {
                ref readonly TargetEntity targetEntity = ref aspect.TargetEntities.Read(entity);

                if (!targetEntity.Value.TryGetID(out int targetID))
                    continue;

                ref readonly UpdateRewardRequest updateRewardRequest = ref aspect.UpdateRewardRequest.Read(entity);

                TargetEntityAspect targetEntityAspect = _world.GetAspect<TargetEntityAspect>();

                ref RewardCollectedAt rewardCollectedAt = ref targetEntityAspect.RewardCollectedAt.Get(targetID);
                ref RewardsCount rewardsCount = ref targetEntityAspect.RewardsCounts.Get(targetID);

                rewardCollectedAt.Value = updateRewardRequest.Time;

                if (updateRewardRequest.Overwrite)
                    rewardsCount.Value = updateRewardRequest.Count;
                else
                    rewardsCount.Value += updateRewardRequest.Count;

                int @event = _world.NewEntity();
                targetEntityAspect.RewardUpdatedEvent.Add(@event);
                targetEntityAspect.TargetEntity.Add(@event).Value = targetEntity.Value;
            }
        }
    }
}