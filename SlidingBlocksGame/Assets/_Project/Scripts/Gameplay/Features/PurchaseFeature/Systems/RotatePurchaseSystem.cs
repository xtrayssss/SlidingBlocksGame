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

        private class PurchaseSnappedAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PurchaseTag))]
            [IncImplicit(typeof(SnappedEvent))]
            [Inc] public readonly EcsPool<PhysicView> PhysicViews;

            [Inc] public readonly EcsPool<RotationTween> RotationTween;
        }

        private class PurchaseLeaveAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PurchaseTag))]
            [IncImplicit(typeof(LeaveEvent))]
            [Inc] public readonly EcsPool<PhysicView> PhysicViews;

            [Inc] public readonly EcsPool<RotationTween> Factors;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out PurchaseSnappedAspect aspect))
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

                _world.GetPool<ViewUpdatedMarker>().TryAdd(entity);
            }

            foreach (int entity in _world.Where(out PurchaseLeaveAspect aspect))
            {
                ref RotationTween rotationTween = ref aspect.Factors.Get(entity);

                ref PhysicView physicView = ref aspect.PhysicViews.Get(entity);

                rotationTween.Value.Stop();

                rotationTween.Value = Tween.LocalEulerAngles(
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