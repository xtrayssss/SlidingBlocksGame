using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;
using YG;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class CanRewardSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class RewardAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(RewardTag))]
            [Inc] public readonly EcsPool<RewardInterval> RewardIntervals;
            [Inc] public readonly EcsPool<RewardCollectedAt> RewardCollectedAt;

            [Opt] public readonly EcsTagPool<CanRewardMarker> CanReward;
        }

        public void Run()
        {
            foreach (int reward in _world.Where(out RewardAspect aspect))
            {
                long lastRewardTime = aspect.RewardCollectedAt.Read(reward).Value;

                long currentTime = YandexGame.ServerTime();

                if (currentTime - lastRewardTime >= aspect.RewardIntervals.Read(reward).Value)
                    aspect.CanReward.TryAdd(reward);
                else
                    aspect.CanReward.TryDel(reward);
            }
        }
    }
}