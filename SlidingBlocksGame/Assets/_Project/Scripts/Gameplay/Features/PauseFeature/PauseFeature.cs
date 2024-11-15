using _Project.Scripts.Gameplay.Features.PauseFeature.Components;
using _Project.Scripts.Gameplay.Features.PauseFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.PauseFeature
{
    public class PauseFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AutoDelTag<PausedEvent>()
                .AddUnique(new PauseSystem())
                .AutoDelEntityTag<PauseRequest>();
        }
    }
}