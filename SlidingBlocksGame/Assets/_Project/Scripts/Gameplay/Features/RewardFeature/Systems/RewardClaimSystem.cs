using _Project.Scripts.Gameplay.Features.CoinFeature.Components;
using _Project.Scripts.Gameplay.Features.CoinFeature.Utils;
using _Project.Scripts.Gameplay.Features.PlayerFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.Utils;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.ButtonFeature.Components;
using DCFApixels.DragonECS;
using YG;

namespace _Project.Scripts.Gameplay.Features.RewardFeature.Systems
{
    public class RewardClaimSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class ClaimRewardAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(RewardTag))]
            [IncImplicit(typeof(RewardEligibilityMarker))]
            [Inc] public readonly EcsPool<RewardScalingCurve> RewardScalingCurve;

            [Opt] public readonly EcsTagPool<RewardCollectedEvent> CollectedEvent;
        }

        private class RewardClickedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<RewardButtonTag> RewardButtonTag;
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> ButtonClickedEvent;
        }

        private class PlayerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayerTag))]
            [Inc] public readonly EcsPool<Coins> Coins;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out RewardClickedAspect _))
            {
                foreach (int reward in _world.Where(out ClaimRewardAspect rewardAspect))
                {
                    foreach (int player in _world.Where(out PlayerAspect _))
                    {
                        int rewardCoins = (int)rewardAspect.RewardScalingCurve.Read(reward).Value
                            .Evaluate(YandexGame.savesData.Savings.RewardCount);

                        CoinUtils.Update(
                            coinable: player,
                            coins: rewardCoins);

                        RewardUtils.Update(
                            rewardable: reward,
                            data: new UpdateRewardRequest
                            {
                                Count = rewardCoins,
                                CollectionTime = YandexGame.ServerTime()
                            });

                        rewardAspect.CollectedEvent.Add(reward);
                    }
                }
            }
        }
    }
}