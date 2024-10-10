using _Project.Scripts.Gameplay.Features.RateUsFeature.Systems;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.RateUsFeature
{
    public class RateUsFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder) => 
            builder
                .AddUnique(new RateUsSystem());
    }
}