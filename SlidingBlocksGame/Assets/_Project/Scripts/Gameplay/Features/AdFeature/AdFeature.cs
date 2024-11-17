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
                
                .AddUnique(new CatchAdEventsSystem())
                .AddUnique(new AdSystem())
                .AutoDelEntityTag<ShowAdRequest>();
        }
    }
}