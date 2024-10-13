using _Project.Scripts.Gameplay.Features.DestructionFeature.Components;
using _Project.Scripts.Gameplay.Features.DestructionFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.DestructionFeature
{
    public class DestructionFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AutoDelTag<DeathEvent>()
                .AddUnique(new DestructionChainStrategySystem())
                .AutoDelEntityTag<ApplyDestructionStrategyRequest>()
                //
                .AutoDelTag<ViewDestroyedEvent>()
                .AddUnique(new DestroyViewSystem())
                .AutoDelEntityTag<DeleteEntityRequest>();
        }
    }
}