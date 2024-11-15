using _Project.Scripts.Gameplay.Features.AdFeature.Components;
using _Project.Scripts.Gameplay.Features.AdFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AdFeature
{
    public class AdFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AutoDelEntityTag<AdCompletedEvent>()
                .AutoDelEntityTag<AdCompletedProcessedMarker>()
                .AddUnique(new AdSystem())
                .AddUnique(new CatchAdEventsSystem())
                .AutoDelEntityTag<ShowAdRequest>();
        }
    }
}