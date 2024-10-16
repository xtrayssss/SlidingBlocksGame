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
            [IncImplicit(typeof(SnappedEvent))]
            [Inc] public readonly EcsPool<PhysicView> PhysicViews;

            [Inc] public readonly EcsPool<PurchaseWidget> PurchaseWidgets;
        }

        private class PurchaseLeaveAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PurchaseTag))]
            [IncImplicit(typeof(LeaveEvent))]
            [Inc] public readonly EcsPool<PhysicView> PhysicViews;

            [Inc] public readonly EcsPool<PurchaseWidget> PurchaseWidgets;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out PurchaseSnappedAspect aspect))
            {
                ref PhysicView physicView = ref aspect.PhysicViews.Get(entity);
                ref PurchaseWidget widget = ref aspect.PurchaseWidgets.Get(entity);

                widget.RotationTween.Stop();

                widget.RotationTween = Tween.LocalEulerAngles(
                    target: physicView.Value.transform,
                    startValue: physicView.Value.transform.eulerAngles,
                    endValue: physicView.Value.transform.eulerAngles + new Vector3(0, 360, 0),
                    duration: 4f,
                    ease: Ease.Linear,
                    cycles: -1,
                    cycleMode: CycleMode.Incremental);

                _world.GetPool<ViewUpdatedMarker>().TryAdd(entity);
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
                    .OnComplete(() => _world.GetPool<ViewUpdatedMarker>().TryDel(entity));
            }
        }
    }
}