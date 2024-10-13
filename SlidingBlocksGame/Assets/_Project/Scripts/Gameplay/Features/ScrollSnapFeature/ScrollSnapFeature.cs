using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Systems;
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Systems._Project.Scripts.Gameplay.Features.ScrollSnapFeature.
    Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature
{
    public class ScrollSnapFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder buidler)
        {
            buidler
                .AddUnique(new SetupScrollSystem())
                .AutoDel<SetupScrollRequest>()
                //
                .AddUnique(new ScrollNearestSystem())
                .AutoDelTag<ScrollNearestRequest>()
                //
                .AddUnique(new ScrollEffectRequestSystem())
                //
                .AddUnique(new ScrollIdleSystem())
                .AutoDelTag<SnappedEvent>()
                .AddUnique(new ScrollToTargetSystem())
                .AutoDelTag<LeaveEvent>()
                .AddUnique(new ScrollDraggingSystem())
                //
                .AddUnique(new ScrollEffectsSystem())
                .AutoDelEntityComponent<ApplyEffectRequest>()
                //
                .AddUnique(new LockUnlockScrollSystem())
                .AutoDelTag<LockScrollSnapRequest>()
                .AutoDelTag<UnlockScrollSnapRequest>()
                //
                .AddUnique(new DynamicLayoutSystem());
        }
    }
}