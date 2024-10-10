using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.Components;
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.PurchaseFeature.Systems
{
    public class RotatePurchaseSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class PurchaseSnappedStateAspect : EcsAspectAuto
        {
            public class OnEnter : EcsAspectAuto
            {
                [IncImplicit(typeof(ScrollSnappedEvent))]
                [IncImplicit(typeof(PurchaseTag))]
                [Inc] public readonly EcsPool<PhysicView> PhysicViews;

                [Inc] public readonly EcsPool<RotationTween> RotationTween;
            }
        }

        private class PurchaseDraggedStateAspect : EcsAspectAuto
        {
            public class OnEnter : EcsAspectAuto
            {
                [IncImplicit(typeof(PurchaseTag))]
                [IncImplicit(typeof(ScrollDraggedEvent))]
                [Inc] public readonly EcsPool<PhysicView> PhysicViews;

                [Inc] public readonly EcsPool<RotationTween> Factors;
            }
        }

        private class ScrollClosedState : EcsAspectAuto
        {
            public class OnEnter : EcsAspectAuto
            {
                [Inc] public readonly EcsPool<ScrollSnap> ScrollSnaps;
            }
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out PurchaseSnappedStateAspect.OnEnter aspect))
            {
                ref RotationTween rotationTween = ref aspect.RotationTween.Get(entity);

                ref PhysicView physicView = ref aspect.PhysicViews.Get(entity);

                rotationTween.Value.Stop();

                rotationTween.Value = Tween.LocalEulerAngles(
                    target: physicView.Value.transform,
                    startValue: physicView.Value.transform.eulerAngles,
                    endValue: physicView.Value.transform.eulerAngles + new Vector3(0, 360, 0),
                    duration: 4f,
                    ease: Ease.Linear,
                    cycles: -1,
                    cycleMode: CycleMode.Incremental);

                rotationTween.Value = Tween
                    .Delay(4f)
                    .OnComplete(() => Debug.Log("DONE"));

                _world.GetPool<ViewUpdatedMarker>().TryAdd(entity);
            }

            foreach (int entity in _world.Where(out PurchaseDraggedStateAspect.OnEnter aspect))
            {
                ref RotationTween rotationTween = ref aspect.Factors.Get(entity);

                ref PhysicView physicView = ref aspect.PhysicViews.Get(entity);

                rotationTween.Value.Stop();

                rotationTween.Value.Stop();

                rotationTween.Value = Tween.LocalEulerAngles(
                        target: physicView.Value.transform,
                        startValue: physicView.Value.transform.eulerAngles,
                        endValue: new Vector3(0, 0, 0),
                        duration: 1f,
                        ease: Ease.Linear)
                    .OnComplete(() => _world.GetPool<ViewUpdatedMarker>().TryDel(entity));
            }

            foreach (int entity in _world.Where(out ScrollClosedState.OnEnter aspect))
            {
                ref ScrollSnap scrollSnap = ref aspect.ScrollSnaps.Get(entity);

                if (!scrollSnap.Selected.TryGetID(out int selectedID))
                    continue;

                ref RotationTween rotationTween = ref _world.GetPool<RotationTween>().Get(selectedID);
                ref PhysicView physicView = ref _world.GetPool<PhysicView>().Get(selectedID);

                rotationTween.Value.Stop();

                physicView.Value.transform.eulerAngles = new Vector3(0, 0, 0);
                //
                // rotationTween.Tween = Tween.LocalEulerAngles(
                //         target: physicView.Value.transform,
                //         startValue: physicView.Value.transform.eulerAngles,
                //         endValue: new Vector3(0, 0, 0),
                //         duration: 1f,
                //         ease: Ease.Linear)
                //     .OnComplete(() => _world.GetPool<ViewUpdatedMarker>().TryDel(entity));
            }
        }
    }
}