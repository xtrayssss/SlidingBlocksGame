using _Project.Scripts.Gameplay.Features.TutorialFeature.IntegrationFeatures.UIFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.TutorialFeature
{
    public class TutorialFeature<TMask> : EcsModule<TMask> where TMask : EcsAspect, new()
    {
        protected override void Import(Builder builder)
        {
            builder
                .AddUnique(new TutorialSystem());
        }
    }
}