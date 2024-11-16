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
                
                .AddUnique(new CatchAdEventsSystem())
                .AddUnique(new AdSystem())
                .AutoDelEntityTag<ShowAdRequest>();
        }
    }
}