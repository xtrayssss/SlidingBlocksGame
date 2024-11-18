using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.MovementFeature
{
    public class MovementFeature<TMask> : EcsModule<TMask> where TMask : EcsAspect, new()
    {
        protected override void Import(Builder builder)
        {
            builder
                .AutoDelEntityTag<ApplyMovementStrategyRequest>()
                .AddSystem(new CalculateDestinationSystem())
                //
                .AddSystem(new CalculateMovementSpeedSystem())
                .AutoDelTag<CalculateMovementSpeedRequest>()
                //
                .AddSystem(new MovementChainStrategySystem())
                .AutoDelEntityTag<MovementTweenCompletedEvent>()
                .AddSystem(new CatchMovementTweenSystem())
                // 
                .AddSystem(new SyncPositionsSystem());
        }
    }
}