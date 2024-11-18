using _Project.Scripts.Gameplay.Features.CameraFeature.Components;
using _Project.Scripts.Gameplay.Features.CameraFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CameraFeature
{
    public class CameraFeature<TMask> : EcsModule<TMask> where TMask : EcsAspect, new()
    {
        protected override void Import(Builder builder)
        {
            builder
                .AddSystem(new CameraRenderSystem())
                .AddSystem(new Object3DPreviewSystem())
                .AutoDelEntityComponent<ObjectPreviewRequest>();
        }
    }
}