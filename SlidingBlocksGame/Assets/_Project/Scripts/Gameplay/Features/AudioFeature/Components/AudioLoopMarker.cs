using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Components
{
    [Serializable]
    public struct AudioLoopMarker : IEcsTagComponent
    {
        private sealed class Template : TagComponentTemplate<AudioLoopMarker>
        {
        }
    }
}