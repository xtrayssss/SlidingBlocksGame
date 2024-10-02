using System;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.DestructionFeature.Components;
using _Project.Scripts.Gameplay.Features.InputFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.InputFeature.Systems
{
    public class InputSystem : IEcsInit, IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class MobileAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<MobileDeviceMarker> MobileDeviceMarker;
            [Exc] public readonly EcsTagPool<LockGameInputMarker> LockGameInputMarker;
        }

        private class StandaloneAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<StandaloneDeviceMarker> StandaloneDeviceMarker;
            [Exc] public readonly EcsTagPool<LockGameInputMarker> LockGameInputMarker;
        }

        private class InputAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsTagPool<ClickDownEvent> ClickDown;
            [Opt] public readonly EcsTagPool<ClickUpEvent> ClickUp;
            [Opt] public readonly EcsTagPool<DeleteEntityRequest> DeleteEntity;
            [Opt] public readonly EcsTagPool<PrimaryClickMarker> PrimaryClick;
            [Opt] public readonly EcsPool<ScreenPosition> ScreenPosition;
        }

        private class PlayerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayerTag))]
            [Opt] public readonly EcsTagPool<StandaloneDeviceMarker> StandaloneDevice;

            [Opt] public readonly EcsTagPool<MobileDeviceMarker> MobileDevice;
        }

        private InputAspect _inputAspect;

        public void Init()
        {
            foreach (int entity in _world.Where(out PlayerAspect playerAspect))
            {
                if (Application.platform == RuntimePlatform.Android)
                    playerAspect.MobileDevice.Add(entity);
                else
                    playerAspect.StandaloneDevice.Add(entity);
            }

            _inputAspect = _world.GetAspect<InputAspect>();
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out MobileAspect _))
            {
                if (Input.touches.Length > 0)
                {
                    ref Touch primaryTouch = ref Input.touches[0];

                    if (primaryTouch.phase == TouchPhase.Began)
                    {
                        int click = CreateClickDown(primaryTouch.position);

                        _inputAspect.PrimaryClick.Add(click);
                    }

                    if (primaryTouch.phase == TouchPhase.Ended)
                        CreateClickUp();

                    int touchCount = Mathf.Min(Input.touchCount, 4);

                    int startSlice = touchCount > 1 ? 1 : 0;

                    foreach (ref readonly Touch touch in Input.touches.AsSpan()
                                 .Slice(startSlice, touchCount - startSlice))
                    {
                        if (touch.phase == TouchPhase.Began)
                            CreateClickDown(touch.position);
                        else if (touch.phase == TouchPhase.Ended)
                            CreateClickUp();
                    }
                }
            }

            foreach (int entity in _world.Where(out StandaloneAspect _))
            {
                if (Input.GetMouseButtonDown(0))
                    CreateClickDown(Input.mousePosition);

                if (Input.GetMouseButtonUp(0))
                    CreateClickUp();
            }
        }

        private void CreateClickUp()
        {
            int click = _world.NewEntity();
            _inputAspect.ClickUp.Add(click);
            _inputAspect.DeleteEntity.Add(click);
        }

        private int CreateClickDown(float2 position)
        {
            int click = _world.NewEntity();
            _inputAspect.ClickDown.Add(click);
            _inputAspect.DeleteEntity.Add(click);
            _inputAspect.ScreenPosition.Add(click).Value = position;

            return click;
        }

        private int CreateClickDown(float3 position)
        {
            int click = _world.NewEntity();
            _inputAspect.ClickDown.Add(click);
            _inputAspect.DeleteEntity.Add(click);
            _inputAspect.ScreenPosition.Add(click).Value = position.xy;

            return click;
        }
    }
}