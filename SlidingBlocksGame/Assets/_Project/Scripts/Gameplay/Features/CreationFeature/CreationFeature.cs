using _Project.Scripts.Gameplay.Features.CreationFeature.Components;
using _Project.Scripts.Gameplay.Features.CreationFeature.Systems;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CreationFeature
{
    public class CreationFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AddUnique(new CreationChainStrategySystem())
                .AutoDelEntityTag<ApplyCreationStrategyRequest>();
        }
    }
}