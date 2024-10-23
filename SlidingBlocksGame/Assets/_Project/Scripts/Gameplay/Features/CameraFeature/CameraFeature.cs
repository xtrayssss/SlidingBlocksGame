using _Project.Scripts.Gameplay.Features.CameraFeature.Components;
using _Project.Scripts.Gameplay.Features.CameraFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CameraFeature
{
    public class CameraFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AddUnique(new CameraRenderSystem())
                .AddUnique(new Render3DToUISystem())
                .AutoDel<Render3DToUIRequest>();
        }
    }
}