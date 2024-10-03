using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Systems;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Systems;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Systems;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.MovementFeature
{
    public class MovementFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AddUnique(new GameFieldSideClickSystem())
                .AutoDelEntityTag<ApplyMovementStrategyRequest>()
                .AddUnique(new DestinationCellSystem())
                //
                .AddUnique(new CalculateMovementSpeedSystem())
                .AutoDelTag<CalculateMovementSpeedRequest>()
                //
                .AddUnique(new MovementChainStrategySystem())
                
                .AutoDelEntityTag<MovementTweenCompletedEvent>()
                .AddUnique(new CatchMovementTweenSystem())
                //
                .AddUnique(new UpdatePositionsSystem())
                .AddUnique(new WithinCenterSystem());
        }
    }
}