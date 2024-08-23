using System;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class RotationSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class AnimalAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalTag))]
            [IncImplicit(typeof(ScrollSnappedMarker))]
            [Inc] public readonly EcsPool<PhysicView> PhysicViews;

            [Inc] public readonly EcsPool<RotationSpeedFactor> Factors;
        }

        private class SnapBackAnimalAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalTag))]
            [IncImplicit(typeof(ScrollStartedMarker))]
            [Inc] public readonly EcsPool<PhysicView> PhysicViews;

            [Inc] public readonly EcsPool<RotationSpeedFactor> Factors;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out AnimalAspect aspect))
            {
                ref RotationSpeedFactor factor = ref aspect.Factors.Get(entity);

                aspect.PhysicViews.Get(entity).Value.transform.Rotate(factor.Value * Vector3.up * Time.deltaTime);

                factor.IsSnapBack = true;

                factor.Original = aspect.PhysicViews.Get(entity).Value.transform.rotation;
                
                _world.GetPool<ViewUpdatedEvent>().Add(entity);
            }

            foreach (int entity in _world.Where(out SnapBackAnimalAspect aspect))
            {
                ref RotationSpeedFactor factor = ref aspect.Factors.Get(entity);

                ref PhysicView physicView = ref aspect.PhysicViews.Get(entity);

                if (factor.IsSnapBack)
                {
                    physicView.Value.transform.rotation = Quaternion.Slerp(physicView.Value.transform.rotation, Quaternion.identity,
                        factor.SnapBackFactor * Time.deltaTime);

                    if (Quaternion.Angle(physicView.Value.transform.rotation, Quaternion.identity) < 0.1f)
                    {
                        physicView.Value.transform.rotation = Quaternion.identity;
                        factor.IsSnapBack = false;
                    }
                }

                _world.GetPool<ViewUpdatedEvent>().Add(entity);
            }
        }
    }

    public class CameraRenderSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ViewUpdatedEvent))]
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
    public struct ViewUpdatedEvent : IEcsTagComponent
    {
    }

    [Serializable]
    public struct RenderCamera : IEcsComponent
    {
        public Camera Value;
    }
}