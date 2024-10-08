using _Project.Scripts.Gameplay.Features.GameProgressFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;
using YG;

namespace _Project.Scripts.Gameplay.Features.GameProgressFeature.Systems
{
    public class RewardClaimSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class RewardButtonClickedAspect : EcsAspectAuto
        {
            [Inc] private readonly EcsTagPool<RewardButtonTag> _rewardButtonTag;
            [Inc] private readonly EcsTagPool<ButtonClickedEvent> _buttonClickedEvent;
        }

        private class RewardAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(RewardTag))]
            [IncImplicit(typeof(RewardEligibilityMarker))]
            [Inc] public readonly EcsPool<RewardScalingCurve> RewardScalingCurve;
        }

        private class PlayerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayerTag))]
            [Inc] public readonly EcsPool<Coins> Coins;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out RewardButtonClickedAspect _))
            {
                foreach (int reward in _world.Where(out RewardAspect rewardAspect))
                {
                    foreach (int player in _world.Where(out PlayerAspect _))
                    {
                        int rewardCoins = (int)rewardAspect.RewardScalingCurve.Read(reward).Value
                            .Evaluate(YandexGame.savesData.RewardCount);

                        ProgressUtils.UpdateCoins(
                            target: player,
                            coins: rewardCoins);

                        ProgressUtils.UpdateReward(
                            target: reward,
                            time: YandexGame.ServerTime(),
                            count: 1);
                    }
                }
            }
        }
    }
}