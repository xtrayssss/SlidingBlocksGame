using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.MovementFeautre.Components
{
    [Serializable]
    public struct MovementSpeedFactor : IEcsComponent
    {
        public float Value;

        [Serializable]
        public sealed class Wrapper : ComponentTemplate<MovementSpeedFactor>
        {
        }
    }
}