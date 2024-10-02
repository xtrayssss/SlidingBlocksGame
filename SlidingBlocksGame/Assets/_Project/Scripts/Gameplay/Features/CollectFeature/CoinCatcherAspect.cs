using _Project.Scripts.Gameplay.Features.CollectFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CollectFeature
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
    }
}