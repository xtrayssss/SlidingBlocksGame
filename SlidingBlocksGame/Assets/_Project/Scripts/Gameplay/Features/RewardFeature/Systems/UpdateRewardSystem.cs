using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.RewardFeature.Systems
{
    public class UpdateRewardSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class RequestAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<UpdateRewardRequest> UpdateRewardRequest;
            [Opt] public readonly EcsPool<TargetEntity> Rewardable;
        }

        private class RewardableAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Reward> Rewards;
        }

        private class EventAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<RewardUpdatedEvent> RewardUpdatedEvent;
            [Inc] public readonly EcsPool<TargetEntity> Rewardable;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out RequestAspect requestAspect))
            {
                ref readonly TargetEntity rewardable = ref requestAspect.Rewardable.Read(entity);

                if (!rewardable.Value.TryGetID(out int rewardableID))
                    continue;

                ref readonly UpdateRewardRequest updateRequest = ref requestAspect.UpdateRewardRequest.Read(entity);

                RewardableAspect rewardableAspect = _world.GetAspect<RewardableAspect>();

                ref Reward reward = ref rewardableAspect.Rewards.Get(rewardableID);

                reward.CollectionTime = updateRequest.CollectionTime;

                if (updateRequest.Overwrite)
                    reward.ClaimedCount = updateRequest.Count;
                else
                    reward.ClaimedCount += updateRequest.Count;

                GenerateEvent(rewardable.Value);
            }
        }

        private void GenerateEvent(entlong rewardable)
        {
            EventAspect eventAspect = _world.GetAspect<EventAspect>();
            int @event = _world.NewEntity();
            eventAspect.RewardUpdatedEvent.Add(@event);
            eventAspect.Rewardable.Add(@event).Value = rewardable;
        }
    }
}