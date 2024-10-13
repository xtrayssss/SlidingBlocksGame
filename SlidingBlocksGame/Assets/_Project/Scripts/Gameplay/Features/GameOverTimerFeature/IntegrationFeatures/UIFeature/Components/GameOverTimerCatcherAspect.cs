using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using DCFApixels.DragonECS;
// ReSharper disable UnassignedReadonlyField

namespace _Project.Scripts.Gameplay.Features.GameOverTimerFeature.IntegrationFeatures.UIFeature.Components
{
    public class GameOverTimerCatcherAspect : EcsAspectAuto
    {
        [Inc] public readonly EcsTagPool<CatchGameOverClosedRequest> CatchGameOverClosed;
        [Opt] public readonly EcsTagPool<GameOverTimerClosedEvent> GameOverTimerClosedEvent;
        
        public CommonCatcherAspect CommonCatcherAspect;

        protected override void InitAfterDI(Builder builder)
        {
            CommonCatcherAspect = builder.Combine<CommonCatcherAspect>();
        }
    }
}