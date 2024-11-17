using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.PauseFeature.Components
{
    
    [Serializable]
    [MetaGroup("Pause")]
    public struct PausedMarker : IEcsTagComponent
    {
        private sealed class Template : TagComponentTemplate<PausedMarker>
        {
        }
    }
}