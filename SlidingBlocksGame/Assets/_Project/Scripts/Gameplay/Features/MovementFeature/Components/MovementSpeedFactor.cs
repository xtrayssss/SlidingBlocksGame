using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Components
{
    [Serializable]
    public struct MovementSpeedFactor : IEcsComponent
    {
        public float Value;

        private sealed class Template : ComponentTemplate<MovementSpeedFactor>
        {
        }
    }
}