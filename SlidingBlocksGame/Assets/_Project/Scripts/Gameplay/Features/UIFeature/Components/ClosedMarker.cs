using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct ClosedMarker : IEcsTagComponent
    {
        private sealed class Template : TagComponentTemplate<ClosedMarker>
        {
        }
    }
}