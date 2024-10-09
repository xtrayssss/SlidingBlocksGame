using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    internal class MovementTweenCatcherAspect : EcsAspectAuto
    {
        [Inc] public readonly EcsTagPool<CatchMovementTweenRequest> CatchMovementTweenRequest;

        public CommonCatcherAspect CommonCatcherAspect;

        protected override void InitAfterDI(Builder builder) =>
            CommonCatcherAspect = builder.Combine<CommonCatcherAspect>();
    }
}