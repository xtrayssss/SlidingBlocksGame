using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Systems;
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Systems._Project.Scripts.Gameplay.Features.ScrollSnapFeature.
    Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature
{
    public class ScrollSnapFeature<TMask> : EcsModule<TMask> where TMask : EcsAspect, new()
    {
        protected override void Import(Builder builder)
        {
            builder
                .AddUnique(new SetupScrollSystem())
                .AutoDel<SetupScrollRequest>()
                //
                .AddUnique(new ScrollNearestSystem())
                .AutoDelTag<ScrollNearestRequest>()
                //
                .AddUnique(new ScrollEffectRequestSystem())
                //
                .AddUnique(new ScrollSnappedSystem())
                .AutoDelTag<SnappedEvent>()
                .AddUnique(new ScrollToTargetSystem())
                .AutoDelTag<LeavedEvent>()
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