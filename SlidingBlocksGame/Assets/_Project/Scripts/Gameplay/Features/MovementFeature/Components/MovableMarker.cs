using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Components
{
    [Serializable]
    [MetaGroup("Movement")]
    public struct MovableMarker : IEcsTagComponent
    {
        private sealed class Template : TagComponentTemplate<MovableMarker>
        {
        }
    }
}