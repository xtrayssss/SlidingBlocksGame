using _Project.Scripts.Gameplay.Features.VisualFeature.VFXFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.VFXFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.VFXFeature
{
    public class VfxFeature<TMask> : EcsModule<TMask> where TMask : EcsAspect, new()
    {
        protected override void Import(Builder builder)
        {
            builder
                .AddSystem(new PlayFxSystem())
                .AutoDelTag<PlayFxRequest>()
                .AddSystem(new DestroyVfxSystem())
                .AddSystem(new DestructionFxSystem());
        }
    }
}