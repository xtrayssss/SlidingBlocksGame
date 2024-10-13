<<<<<<<< HEAD:SlidingBlocksGame/Assets/_Project/Scripts/Gameplay/Features/CollectFeature/RewardCollectSystem.cs
using _Project.Scripts.Gameplay.Features.CollectFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;
using YG;

namespace _Project.Scripts.Gameplay.Features.CollectFeature
{
    public class RewardCollectSystem : IEcsRun
========
﻿using _Project.Scripts.Gameplay.Features.GameProgressFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.ButtonFeature.Components;
using DCFApixels.DragonECS;
using YG;

namespace _Project.Scripts.Gameplay.Features.RewardFeature.Systems
{
    public class RewardClaimSystem : IEcsRun
>>>>>>>> recovery-branch:SlidingBlocksGame/Assets/_Project/Scripts/Gameplay/Features/RewardFeature/Systems/RewardClaimSystem.cs
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
<<<<<<<< HEAD:SlidingBlocksGame/Assets/_Project/Scripts/Gameplay/Features/CollectFeature/RewardCollectSystem.cs
            [IncImplicit(typeof(CanRewardMarker))]
            [Inc] public readonly EcsPool<CoinsProgressionCurve> CoinsProgressionCurves;

            [Inc] public readonly EcsPool<RewardsCount> RewardsCount;
========
            [IncImplicit(typeof(RewardEligibilityMarker))]
            [Inc] public readonly EcsPool<RewardScalingCurve> RewardScalingCurve;
>>>>>>>> recovery-branch:SlidingBlocksGame/Assets/_Project/Scripts/Gameplay/Features/RewardFeature/Systems/RewardClaimSystem.cs
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
                        int rewardCoins = (int)rewardAspect.CoinsProgressionCurves.Read(reward).Value
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