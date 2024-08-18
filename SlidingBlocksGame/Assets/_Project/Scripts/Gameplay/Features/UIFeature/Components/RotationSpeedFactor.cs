using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct RotationSpeedFactor : IEcsComponent
    {
        public float Value;
        public Quaternion Original;
        public float SnapBackFactor;
        public bool IsSnapBack;

        private sealed class Template : ComponentTemplate<RotationSpeedFactor>
        {
        }
    }
}