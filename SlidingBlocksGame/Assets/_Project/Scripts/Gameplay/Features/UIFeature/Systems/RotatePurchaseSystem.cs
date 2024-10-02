using System;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class RotatePurchaseSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class AnimalSnappedStateAspect : EcsAspectAuto
        {
            public class OnEnter : EcsAspectAuto
            {
                [IncImplicit(typeof(ScrollSnappedEvent))]
                [IncImplicit(typeof(PurchaseAnimalTag))]
                [Inc] public readonly EcsPool<PhysicView> PhysicViews;

                [Inc] public readonly EcsPool<RotationTween> RotationTween;
            }
        }

        private class AnimalDraggedStateAspect : EcsAspectAuto
        {
            public class OnEnter : EcsAspectAuto
            {
                [IncImplicit(typeof(PurchaseAnimalTag))]
                [IncImplicit(typeof(ScrollDraggedEvent))]
                [Inc] public readonly EcsPool<PhysicView> PhysicViews;

                [Inc] public readonly EcsPool<RotationTween> Factors;
            }
        }

        private class ScrollClosedState : EcsAspectAuto
        {
            public class OnEnter : EcsAspectAuto
            {
                [IncImplicit(typeof(ClosedEvent))]
                [Inc] public readonly EcsPool<ScrollSnap> ScrollSnaps;
            }
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out AnimalSnappedStateAspect.OnEnter aspect))
            {
                ref RotationTween rotationTween = ref aspect.RotationTween.Get(entity);

                ref PhysicView physicView = ref aspect.PhysicViews.Get(entity);

                rotationTween.Tween.Stop();

                rotationTween.Tween = Tween.LocalEulerAngles(
                    target: physicView.Value.transform,
                    startValue: physicView.Value.transform.eulerAngles,
                    endValue: physicView.Value.transform.eulerAngles + new Vector3(0, 360, 0),
                    duration: 4f,
                    ease: Ease.Linear,
                    cycles: -1,
                    cycleMode: CycleMode.Incremental);

                rotationTween.Delay = Tween.Delay(4f).OnComplete(() => Debug.Log("DONE"));
                
                _world.GetPool<ViewUpdatedMarker>().TryAdd(entity);
            }

            foreach (int entity in _world.Where(out AnimalDraggedStateAspect.OnEnter aspect))
            {
                ref RotationTween rotationTween = ref aspect.Factors.Get(entity);

                ref PhysicView physicView = ref aspect.PhysicViews.Get(entity);

                rotationTween.Tween.Stop();

                rotationTween.Delay.Stop();

                rotationTween.Tween = Tween.LocalEulerAngles(
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

                rotationTween.Tween.Stop();

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

    public struct ClosedEvent : IEcsTagComponent
    {
    }

    public class CameraRenderSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ViewUpdatedMarker))]
            [Inc] public readonly EcsPool<RenderCamera> RenderCameras;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref readonly RenderCamera renderCamera = ref aspect.RenderCameras.Read(entity);

                renderCamera.Value.Render();
            }
        }
    }

    [Serializable]
    public struct ViewUpdatedMarker : IEcsTagComponent
    {
    }

    [Serializable]
    public struct RenderCamera : IEcsComponent
    {
        public Camera Value;
    }
}