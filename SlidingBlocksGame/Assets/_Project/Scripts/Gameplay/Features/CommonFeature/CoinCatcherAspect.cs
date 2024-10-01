using _Project.Scripts.Gameplay.Features.CollectFeature;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CommonFeature
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