using _Project.Scripts.Gameplay.Features.VisualFeature.VFXFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.VFXFeature.Systems;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.VFXFeature
{
    public class VfxFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AddUnique(new PlayFxSystem())
                .AutoDelTag<PlayFxRequest>()
                .AddUnique(new DestroyVfxSystem())
                .AddUnique(new DestructionFxSystem());
        }
    }
}