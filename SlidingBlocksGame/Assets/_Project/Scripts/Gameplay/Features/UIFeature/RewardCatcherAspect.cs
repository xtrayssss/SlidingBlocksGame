using _Project.Scripts.Gameplay.Features.MovementFeature;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Systems;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature
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

        public class CoinAddedToTextCatcher : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CatchCoinAddedToTextRequest> CatchCoinAddedToTextRequest;
            [Opt] public readonly EcsTagPool<CoinAddedToTextEvent> CoinAddedToTextEvent;

            public CommonCatcherAspect CommonCatcherAspect;

            protected override void InitAfterDI(Builder builder) =>
                CommonCatcherAspect = builder.Combine<CommonCatcherAspect>();
        }
        public class RewardCollectedCatcher : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CatchRewardCollectedRequest> CatchRewardCollectedRequest;
            [Opt] public readonly EcsTagPool<RewardCollectedEvent> RewardCollectedEvent;

            public CommonCatcherAspect CommonCatcherAspect;

            protected override void InitAfterDI(Builder builder) =>
                CommonCatcherAspect = builder.Combine<CommonCatcherAspect>();
        }
    }
}