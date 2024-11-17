using _Project.Scripts.Gameplay.Features.PauseFeature.Components;
using _Project.Scripts.Gameplay.Features.PauseFeature.Systems;
using _Project.Scripts.Infrastructure;

namespace _Project.Scripts.Gameplay.Features.PauseFeature
{
    public class PauseFeature : EcsModule
    {
        protected override void Import(Builder builder)
        {
            builder
                .AutoDelTag<PausedEvent>()
                .AddUnique(new PauseSystem())
                .AutoDelEntityTag<PauseRequest>()
                .AutoDelEntityTag<UnpauseRequest>();
        }
    }
}