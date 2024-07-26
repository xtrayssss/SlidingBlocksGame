using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Components
{
    [Serializable]
    public struct ChainMovementCooldown : IEcsComponent
    {
        public float Duration;

        [Serializable]
        public sealed class Wrapper : ComponentTemplate<ChainMovementCooldown>
        {
        }
    }
}