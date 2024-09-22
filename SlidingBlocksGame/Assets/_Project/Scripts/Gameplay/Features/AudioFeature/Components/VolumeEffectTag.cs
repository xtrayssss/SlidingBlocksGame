using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Components
{
    [Serializable]
    [MetaGroup("Audio")]
    public struct VolumeEffectTag : IEcsTagComponent
    {
        private sealed class Template : TagComponentTemplate<VolumeEffectTag>
        {
        }
    }
}