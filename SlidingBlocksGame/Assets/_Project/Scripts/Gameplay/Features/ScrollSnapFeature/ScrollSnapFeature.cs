using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Systems;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature
{
    public class ScrollSnapFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder buidler)
        {
            buidler
                .AddUnique(new ScrollSnapSystem())
                .AutoDel<ScrollSetupRequest>()
                .AutoDelTag<LockScrollSnapRequest>()
                .AutoDelTag<UnlockScrollSnapRequest>()
                .AutoDelTag<ApplyEffectRequest>()
                .AddUnique(new DynamicLayoutSystem());
        }
    }
}