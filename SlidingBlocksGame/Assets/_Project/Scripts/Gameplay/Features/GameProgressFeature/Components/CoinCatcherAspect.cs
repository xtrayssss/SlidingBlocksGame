using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using DCFApixels.DragonECS;
// ReSharper disable UnassignedReadonlyField

namespace _Project.Scripts.Gameplay.Features.GameProgressFeature.Components
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