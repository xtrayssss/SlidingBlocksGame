using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct RotationSpeedFactor : IEcsComponent
    {
        public float Value;

        private sealed class Template : ComponentTemplate<RotationSpeedFactor>
        {
        }
    }
}