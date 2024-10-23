using _Project.Scripts.Gameplay.Features.TutorialFeature.IntegrationFeatures.UIFeature.Systems;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.TutorialFeature
{
    public class TutorialFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AddUnique(new TutorialSystem());
        }
    }
}