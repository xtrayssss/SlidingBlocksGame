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
        private readonly ICoroutineRunner _coroutineRunner;

        public ScrollSnapFeature(ICoroutineRunner coroutineRunner) => 
            _coroutineRunner = coroutineRunner;

        public void Import(EcsPipeline.Builder buidler)
        {
            buidler
                .AddUnique(new SetupScrollSystem())
                .AutoDel<SetupScrollRequest>()
                //
                .AddUnique(new ScrollSnapSystem())
                .AutoDelTag<ScrollUpdateRequest>()
                //
                .AddUnique(new ScrollEffectRequestSystem())
                //
                .AutoDelEntityComponent<SnapToItemEvent>()
                .AutoDelEntityComponent<LeaveItemEvent>()
                .AddUnique(new ScrollIdleSystem())
                .AddUnique(new ScrollToTargetSystem())
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