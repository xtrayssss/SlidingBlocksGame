using _Project.Scripts.Gameplay.Features.CommonFeature;
using _Project.Scripts.Gameplay.Features.MovementFeature;
using _Project.Scripts.Gameplay.Features.RewardFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using DCFApixels.DragonECS;
// ReSharper disable UnassignedReadonlyField

namespace _Project.Scripts.Gameplay.Features.RewardFeature.Systems
{
    public class RewardCatcherAspect
    {
        public class ConfettiCatcher : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CatchConfettiExplodedRequest> CatchConfettiExplodedRequest;
            [Opt] public readonly EcsTagPool<ConfettiExplodedEvent> ConfettiExplodedEvent;

            public CommonCatcherAspect CommonCatcherAspect;

            protected override void InitAfterDI(Builder builder) =>
                CommonCatcherAspect = builder.Combine<CommonCatcherAspect>();
        }

        public class CoinCountDisplayedCatcher : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CatchRewardCoinCountDisplayedRequest> CatchRewardCoinCountDisplayedRequest;
            [Opt] public readonly EcsTagPool<RewardCoinCountDisplayedEvent> RewardCoinCountDisplayedEvent;

            public CommonCatcherAspect CommonCatcherAspect;

            protected override void InitAfterDI(Builder builder) =>
                CommonCatcherAspect = builder.Combine<CommonCatcherAspect>();
        }

        public class CoinDisplayCompletedCatcher : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CatchRewardCoinDisplayCompletedRequest>
                CatchRewardCoinDisplayCompletedRequest;

            [Opt] public readonly EcsTagPool<RewardCoinDisplayCompletedEvent> RewardCoinDisplayCompletedEvent;

            public CommonCatcherAspect CommonCatcherAspect;

            protected override void InitAfterDI(Builder builder) =>
                CommonCatcherAspect = builder.Combine<CommonCatcherAspect>();
        }
    }
}