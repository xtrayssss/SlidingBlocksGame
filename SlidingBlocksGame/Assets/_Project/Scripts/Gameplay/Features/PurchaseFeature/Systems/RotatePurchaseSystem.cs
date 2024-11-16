using _Project.Scripts.Gameplay.Features.CameraFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.PurchaseFeature.Systems
{
    public class RotatePurchaseSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class PurchaseSnappedAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PurchaseTag))]
            [IncImplicit(typeof(SnappedState))]
            [IncImplicit(typeof(UnlockedMarker))]
            [Inc] public readonly EcsPool<PhysicView> PhysicViews;

            [Exc] public readonly EcsTagPool<RotatingMarker> IsRotating;

            [Inc] public readonly EcsPool<PurchaseWidget> PurchaseWidgets;
        }

        private class PurchaseLeaveAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PurchaseTag))]
            [IncImplicit(typeof(LeavedEvent))]
            [IncImplicit(typeof(UnlockedMarker))]
            [Inc] public readonly EcsPool<PhysicView> PhysicViews;

            [Inc] public readonly EcsPool<PurchaseWidget> PurchaseWidgets;
        }

        private class IsRotatingAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<RotatingMarker> IsRotating;
            [Opt] public readonly EcsTagPool<LockedMarker> LockedMarker;
            [Opt] public readonly EcsTagPool<LeavedEvent> LeavedEvent;
        }

        private class PurchaseLockedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<PurchaseTag> PurchaseTag;
            [Inc] public readonly EcsTagPool<LockedMarker> LockedMarker;
            [Inc] public readonly EcsTagPool<SnappedState> SnappedState;
            [Inc] public readonly EcsTagPool<RotatingMarker> IsRotating;
        }

        public void Run()
        {
            foreach (int purchase in _world.Where(out PurchaseLockedAspect _))
            {
                ref PurchaseWidget widget = ref _world.GetPool<PurchaseWidget>().Get(purchase);

                widget.RotationTween.Stop();
            }

            foreach (int purchase in _world.Where(out IsRotatingAspect isRotatingAspect))
            {
                if (isRotatingAspect.LockedMarker.Has(purchase) || isRotatingAspect.LeavedEvent.Has(purchase))
                    isRotatingAspect.IsRotating.Del(purchase);
            }

            foreach (int entity in _world.Where(out PurchaseSnappedAspect aspect))
            {
                ref PhysicView physicView = ref aspect.PhysicViews.Get(entity);
                ref PurchaseWidget widget = ref aspect.PurchaseWidgets.Get(entity);

                widget.RotationTween.Stop();

                widget.RotationTween = Tween.LocalEulerAngles(
                    target: physicView.Value.transform,
                    startValue: physicView.Value.transform.eulerAngles,
                    endValue: new Vector3(0, 360, 0),
                    duration: 4f,
                    ease: Ease.Linear,
                    cycles: -1,
                    cycleMode: CycleMode.Incremental);

                _world.GetPool<RotatingMarker>().Add(entity);
                _world.GetPool<RenderingMarker>().TryAdd(entity);
            }

            foreach (int entity in _world.Where(out PurchaseLeaveAspect aspect))
            {
                ref PhysicView physicView = ref aspect.PhysicViews.Get(entity);

                ref PurchaseWidget widget = ref aspect.PurchaseWidgets.Get(entity);

                widget.RotationTween.Stop();

                widget.RotationTween = Tween.LocalEulerAngles(
                        target: physicView.Value.transform,
                        startValue: physicView.Value.transform.eulerAngles,
                        endValue: new Vector3(0, 0, 0),
                        duration: 1f,
                        ease: Ease.Linear)
                    .OnComplete(() => _world.GetPool<RenderingMarker>().TryDel(entity));
            }
        }
    }
}