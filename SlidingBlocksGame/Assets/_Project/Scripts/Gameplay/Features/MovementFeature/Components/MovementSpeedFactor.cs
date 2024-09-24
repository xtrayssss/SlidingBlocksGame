using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Components
{
    [Serializable]
    [MetaGroup("Movement")]
    public struct MovementSpeedFactor : IEcsComponent
    {
        public float Base;
        public float Value;

        private sealed class Template : ComponentTemplate<MovementSpeedFactor>
        {
        }
    }
}