using _Project.Scripts.Gameplay.Features.AdFeature.Components;
using _Project.Scripts.Gameplay.Features.AdFeature.Systems;
using _Project.Scripts.Infrastructure;

namespace _Project.Scripts.Gameplay.Features.AdFeature
{
    public class AdFeature : EcsModule
    {
        protected override void Import(Builder builder)
        {
            builder
                .AutoDelEntityTag<AdCompletedEvent>()
                
                .AddSystem(new CatchAdEventsSystem())
                .AddSystem(new AdSystem())
                .AutoDelEntityTag<ShowAdRequest>();
        }
    }
}