using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.InputFeature.Components
{
    [Serializable]
    [MetaGroup("Input")]
    public struct LockGameInputMarker : IEcsTagComponent
    {
        private sealed class Template : TagComponentTemplate<LockGameInputMarker>
        {
        }
    }
}