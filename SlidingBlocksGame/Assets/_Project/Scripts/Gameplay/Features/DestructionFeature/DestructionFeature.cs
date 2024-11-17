using _Project.Scripts.Gameplay.Features.DestructionFeature.Components;
using _Project.Scripts.Gameplay.Features.DestructionFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.DestructionFeature
{
    public class DestructionFeature<TMask> : EcsModule<TMask> where TMask : EcsAspect, new()
    {
        protected override void Import(Builder builder)
        {
            builder
                .AutoDelTag<DestructibleStrategyCompletedEvent>()
                .AddUnique(new DestructionChainStrategySystem())
                .AutoDelEntityTag<ApplyDestructionStrategyRequest>()
                //
                .AutoDelTag<ViewDestroyedEvent>()
                .AddUnique(new DestroyViewSystem())
                .AutoDelEntityTag<DeleteEntityRequest>();
        }
    }
}