using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.MovementFeature
{
    internal class MovementTweenCatcherAspect : EcsAspectAuto
    {
        [Inc] public readonly EcsTagPool<CatchMovementTweenRequest> CatchMovementTweenRequest;

        public CommonCatcherAspect CommonCatcherAspect;

        protected override void InitAfterDI(Builder builder) =>
            CommonCatcherAspect = builder.Combine<CommonCatcherAspect>();
    }
}