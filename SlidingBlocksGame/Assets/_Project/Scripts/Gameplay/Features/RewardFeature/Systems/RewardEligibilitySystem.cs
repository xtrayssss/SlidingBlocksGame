using _Project.Scripts.Gameplay.Features.RewardFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;
using YG;

namespace _Project.Scripts.Gameplay.Features.RewardFeature.Systems
{
    public class RewardEligibilitySystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class RewardAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(RewardTag))]
            [Inc] public readonly EcsPool<Reward> Rewards;

            [Opt] public readonly EcsTagPool<RewardEligibilityMarker> RewardEligibilityMarker;
            [Opt] public readonly EcsTagPool<RewardEligibilityEvent> RewardEligibilityEvent;
        }

        public void Run()
        {
            foreach (int rewardID in _world.Where(out RewardAspect rewardAspect))
            {
                ref readonly Reward reward = ref rewardAspect.Rewards.Read(rewardID);

                long currentTime = YandexGame.ServerTime();

                if (currentTime - reward.CollectionTime >= reward.Interval)
                {
                    if (!rewardAspect.RewardEligibilityMarker.Has(rewardID))
                    {
                        rewardAspect.RewardEligibilityMarker.TryAdd(rewardID);
                        rewardAspect.RewardEligibilityEvent.TryAdd(rewardID);
                    }
                }
                else
                    rewardAspect.RewardEligibilityMarker.TryDel(rewardID);
            }
        }
    }
}