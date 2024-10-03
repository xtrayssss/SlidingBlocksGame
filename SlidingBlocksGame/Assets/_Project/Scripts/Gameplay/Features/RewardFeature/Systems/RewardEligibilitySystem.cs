using _Project.Scripts.Gameplay.Features.RewardFeature.Components;
using DCFApixels.DragonECS;
using YG;

namespace _Project.Scripts.Gameplay.Features.RewardFeature.Systems
{
    public class RewardEligibilitySystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class RewardAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(RewardTag))]
            [Inc] public readonly EcsPool<RewardInterval> RewardIntervals;

            [Inc] public readonly EcsPool<RewardCollectionTime> RewardCollectionTime;

            [Opt] public readonly EcsTagPool<RewardEligibilityMarker> RewardEligibilityMarker;
            [Opt] public readonly EcsTagPool<RewardEligibilityEvent> RewardEligibilityEvent;
        }

        public void Run()
        {
            foreach (int reward in _world.Where(out RewardAspect aspect))
            {
                long lastRewardTime = aspect.RewardCollectionTime.Read(reward).Value;

                long currentTime = YandexGame.ServerTime();

                if (currentTime - lastRewardTime >= aspect.RewardIntervals.Read(reward).Value)
                {
                    if (!aspect.RewardEligibilityMarker.Has(reward))
                    {
                        aspect.RewardEligibilityMarker.TryAdd(reward);
                        aspect.RewardEligibilityEvent.TryAdd(reward);
                    }
                }
                else
                    aspect.RewardEligibilityMarker.TryDel(reward);
            }
        }
    }
}