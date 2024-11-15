using _Project.Scripts.Gameplay.Features.AdFeature.Components;
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

        private class RewardClickedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<RewardButtonTag> RewardButtonTag;
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> ButtonClickedEvent;
        }

        private class RewardAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(RewardTag))]
            [IncImplicit(typeof(RewardEligibilityMarker))]
            [Inc] public readonly EcsPool<RewardScalingCurve> RewardScalingCurve;

            [Opt] public readonly EcsTagPool<RewardCollectedEvent> CollectedEvent;
            [Opt] public readonly EcsTagPool<RewardClaimRequest> Claim;
        }
        private class ClaimRewardAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(RewardTag))]
            [IncImplicit(typeof(RewardEligibilityMarker))]
            [IncImplicit(typeof(RewardClaimRequest))]
            [Inc] public readonly EcsPool<RewardScalingCurve> RewardScalingCurve;

            [Opt] public readonly EcsTagPool<RewardCollectedEvent> CollectedEvent;
            [Opt] public readonly EcsTagPool<RewardClaimRequest> Claim;
        }

        private class PlayerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayerTag))]
            [Inc] public readonly EcsPool<Coins> Coins;
        }

        private class ShowAdRequestAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<ShowAdRequest> ShowAd;
        }

        private class AdCompletedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<AdCompletedEvent> AdClosedEvent;
            [Inc] public readonly EcsTagPool<AdCompletedProcessedMarker> AdCompletedProcessedMarker;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out RewardClickedAspect _)) 
                ShowAdd();
            
            foreach (int _ in _world.Where(out AdCompletedAspect _))
            {
                foreach (int reward in _world.Where(out RewardAspect rewardAspect)) 
                    rewardAspect.Claim.Add(reward);
            }

            foreach (int reward in _world.Where(out ClaimRewardAspect rewardAspect))
            {
                foreach (int player in _world.Where(out PlayerAspect _))
                {
                    int rewardCoins = (int)rewardAspect.RewardScalingCurve.Read(reward).Value
                        .Evaluate(YandexGame.savesData.RewardCount);

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

        private void ShowAdd()
        {
            int ad = _world.NewEntity();

            ShowAdRequestAspect requestAspect = _world.GetAspect<ShowAdRequestAspect>();
            requestAspect.ShowAd.Add(ad);
        }
    }
}