<<<<<<<< HEAD:SlidingBlocksGame/Assets/_Project/Scripts/Gameplay/Features/CommonFeature/CoinCatcherAspect.cs
using _Project.Scripts.Gameplay.Features.CollectFeature;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature;
========
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
>>>>>>>> recovery-branch:SlidingBlocksGame/Assets/_Project/Scripts/Gameplay/Features/GameProgressFeature/Components/CoinCatcherAspect.cs
using DCFApixels.DragonECS;
// ReSharper disable UnassignedReadonlyField

<<<<<<<< HEAD:SlidingBlocksGame/Assets/_Project/Scripts/Gameplay/Features/CommonFeature/CoinCatcherAspect.cs
namespace _Project.Scripts.Gameplay.Features.CommonFeature
========
namespace _Project.Scripts.Gameplay.Features.GameProgressFeature.Components
>>>>>>>> recovery-branch:SlidingBlocksGame/Assets/_Project/Scripts/Gameplay/Features/GameProgressFeature/Components/CoinCatcherAspect.cs
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