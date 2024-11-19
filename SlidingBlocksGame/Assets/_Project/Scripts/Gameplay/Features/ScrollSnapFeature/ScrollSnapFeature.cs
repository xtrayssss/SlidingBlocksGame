using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature
{
    public class ScrollSnapFeature<TMask> : EcsModule<TMask> where TMask : EcsAspect, new()
    {
        protected override void Import(Builder builder)
        {
            builder
                .AddSystem(new SetupScrollSystem())
                .AutoDel<SetupScrollRequest>()
                //
                .AddSystem(new ScrollNearestSystem())
                .AutoDelTag<ScrollNearestRequest>()
                //
                .AddSystem(new ScrollEffectRequestSystem())
                //
                .AddSystem(new ScrollSnappedSystem())
                .AutoDelTag<SnappedEvent>()
                .AddSystem(new ScrollToTargetSystem())
                .AutoDelTag<LeavedEvent>()
                .AddSystem(new ScrollDraggingSystem())
                //
                .AddSystem(new ScrollEffectsSystem())
                .AutoDelEntityComponent<ApplyEffectRequest>()
                //
                .AddSystem(new LockUnlockScrollSystem())
                .AutoDelTag<LockScrollSnapRequest>()
                .AutoDelTag<UnlockScrollSnapRequest>()
                //
                .AddSystem(new DynamicLayoutSystem());
        }
    }
}