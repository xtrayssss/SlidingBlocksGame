using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.DestroyFeature.Components
{
    [Serializable]
    public struct CanClickGameFieldMarker : IEcsTagComponent
    {
        private sealed class Template : TagComponentTemplate<CanClickGameFieldMarker>
        {
        }
    }
}