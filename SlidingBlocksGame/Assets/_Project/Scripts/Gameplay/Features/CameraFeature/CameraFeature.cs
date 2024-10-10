using _Project.Scripts.Gameplay.Features.CameraFeature.Systems;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CameraFeature
{
    public class CameraFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AddUnique(new CameraRenderSystem());
        }
    }
}