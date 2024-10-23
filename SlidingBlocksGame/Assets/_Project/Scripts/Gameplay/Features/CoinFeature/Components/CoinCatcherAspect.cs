using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using DCFApixels.DragonECS;

// ReSharper disable UnassignedReadonlyField

namespace _Project.Scripts.Gameplay.Features.CoinFeature.Components
{
    public class CoinCatcherAspect
    {
        public class CoinSpawnedCatcher : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CatchCoinSpawnedRequest> CatchCoinSpawnedRequest;
            [Opt] public readonly EcsTagPool<CoinSpawnedEvent> CoinSpawnedEvent;

            public CommonCatcherAspect CommonCatcherAspect;

            protected override void InitAfterDI(Builder builder) =>
                CommonCatcherAspect = builder.Combine<CommonCatcherAspect>();
        }

        public class CoinCollectAnimationCompletedCatcher : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CatchCoinCollectAnimationCompletedRequest>
                CatchCoinCollectAnimationCompletedRequest;

            [Opt] public readonly EcsTagPool<CoinCollectAnimationCompletedEvent> CoinCollectAnimationCompletedEvent;

            public CommonCatcherAspect CommonCatcherAspect;

            protected override void InitAfterDI(Builder builder) =>
                CommonCatcherAspect = builder.Combine<CommonCatcherAspect>();
        }

        public class CoinDestroyAnimationCompletedCatcher : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CatchCoinDestroyAnimationCompletedRequest>
                CatchCoinDestroyAnimationCompletedRequest;

            [Opt] public readonly EcsTagPool<CoinDestroyAnimationCompletedEvent> CoinDestroyAnimationCompletedEvent;

            public CommonCatcherAspect CommonCatcherAspect;

            protected override void InitAfterDI(Builder builder) =>
                CommonCatcherAspect = builder.Combine<CommonCatcherAspect>();
        }
    }
}