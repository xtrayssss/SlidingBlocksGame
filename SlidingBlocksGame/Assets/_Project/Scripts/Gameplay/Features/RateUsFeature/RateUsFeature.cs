using _Project.Scripts.Gameplay.Features.RateUsFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.RateUsFeature
{
    public class RateUsFeature<TMask> : EcsModule<TMask> where TMask : EcsAspect, new()
    {
        protected override void Import(Builder builder) => 
            builder
                .AddUnique(new RateUsSystem());
    }
}