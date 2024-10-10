using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.CameraFeature.Components
{
    [Serializable]
    public struct RenderCamera : IEcsComponent
    {
        public Camera Value;
    }
}