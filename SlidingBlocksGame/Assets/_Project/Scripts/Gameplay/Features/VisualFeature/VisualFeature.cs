using _Project.Scripts.Gameplay.Features.VisualFeature.VFXFeature;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.VisualFeature
{
    public class VisualFeature<TMask> : EcsModule<TMask> where TMask : EcsAspect, new()
    {
        protected override void Import(Builder builder)
        {
            builder
                .AddSubmodule(new CameraFeature.CameraFeature<TMask>())
                .AddSubmodule(new VfxFeature<TMask>())
                .AddSubmodule(new UIFeature.UIFeature<TMask>());
        }
    }
}