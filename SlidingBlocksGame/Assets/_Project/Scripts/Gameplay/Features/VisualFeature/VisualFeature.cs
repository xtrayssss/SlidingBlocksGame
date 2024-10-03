using _Project.Scripts.Gameplay.Features.VisualFeature.VFXFeature;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.VisualFeature
{
    public class VisualFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AddModule(new VfxFeature())
                .AddModule(new UIFeature.UIFeature());
        }
    }
}