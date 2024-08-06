using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.Components
{
    [Serializable]
    public struct DestroyAfterPlaybackMarker : IEcsTagComponent
    {
        private sealed class Template : TagComponentTemplate<DestroyAfterPlaybackMarker>
        {
        }
    }
}