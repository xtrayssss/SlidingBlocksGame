using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.MovementFeature
{
    public class MovementFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AutoDelEntityTag<ApplyMovementStrategyRequest>()
                .AddUnique(new CalculateDestinationSystem())
                //
                .AddUnique(new CalculateMovementSpeedSystem())
                .AutoDelTag<CalculateMovementSpeedRequest>()
                //
                .AddUnique(new MovementChainStrategySystem())
                .AutoDelEntityTag<MovementTweenCompletedEvent>()
                .AddUnique(new CatchMovementTweenSystem())
                // 
                .AddUnique(new WobbleSystem())
                .AutoDelTag<WobbleRequest>()
                //
                .AddUnique(new UpdatePositionsSystem());
        }
    }
}