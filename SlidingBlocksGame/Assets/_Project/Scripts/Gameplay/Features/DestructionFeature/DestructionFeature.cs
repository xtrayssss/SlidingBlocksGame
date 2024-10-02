using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.DestructionFeature.Components;
using _Project.Scripts.Gameplay.Features.DestructionFeature.Systems;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

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