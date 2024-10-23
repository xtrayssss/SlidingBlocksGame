using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.VFXFeature.Components
{
    [Serializable]
    [MetaGroup("VFX")]
    public struct DestroyAfterPlaybackMarker : IEcsTagComponent
    {
        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.CommonFeature.Components",
            sourceClassName: "DestroyAfterPlaybackMarker/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : TagComponentTemplate<DestroyAfterPlaybackMarker>
        {
        }
    }
}