using System;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.InputFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;
using YG;

namespace _Project.Scripts.Gameplay.Features.PlayerFeature.InputFeature.Systems
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
            [Opt] public readonly EcsPool<ScreenPosition> ScreenPosition;
            [Opt] public readonly EcsTagPool<EmitInputTag> EmitInputTag;
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
                if (YandexGame.EnvironmentData.isMobile || YandexGame.EnvironmentData.isTablet ||
                    UnityEngine.Device.SystemInfo.deviceType != DeviceType.Desktop)
                    playerAspect.MobileDevice.Add(entity);
                else
                    playerAspect.StandaloneDevice.Add(entity);
            }

            _inputAspect = _world.GetAspect<InputAspect>();
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out MobileAspect _))
            {
                foreach (ref readonly Touch touch in Input.touches.AsSpan())
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        CreateClickDown(touch.position);
#if DEBUG
                        Debug.Log("CreateClickDown");
#endif
                    }
                }
            }

            foreach (int _ in _world.Where(out StandaloneAspect _))
            {
                if (Input.GetMouseButtonDown(0))
                    CreateClickDown(Input.mousePosition);
            }
        }

        private int CreateClickDown(float2 position)
        {
            int click = _world.NewEntity();
            _inputAspect.ClickDown.Add(click);
            _inputAspect.ScreenPosition.Add(click).Value = position;
            _inputAspect.EmitInputTag.Add(click);

            return click;
        }

        private int CreateClickDown(float3 position)
        {
            int click = _world.NewEntity();
            _inputAspect.ClickDown.Add(click);
            _inputAspect.ScreenPosition.Add(click).Value = position.xy;
            _inputAspect.EmitInputTag.Add(click);

            return click;
        }
    }
}