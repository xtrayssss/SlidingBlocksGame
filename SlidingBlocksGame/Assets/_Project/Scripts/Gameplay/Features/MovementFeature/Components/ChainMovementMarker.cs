using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Components
{
    [Serializable]
    [MetaGroup("Movement")]
    public struct ChainMovementMarker : IEcsTagComponent
    {
        [Serializable]
        public sealed class Wrapper : TagComponentTemplate<ChainMovementMarker>
        {
        }
    }
}