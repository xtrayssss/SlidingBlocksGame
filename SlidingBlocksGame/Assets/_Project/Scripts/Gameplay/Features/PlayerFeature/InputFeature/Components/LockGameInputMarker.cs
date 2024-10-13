using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.PlayerFeature.InputFeature.Components
{
    [Serializable]
    [MetaGroup("Input")]
    public struct LockGameInputMarker : IEcsTagComponent
    {
        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.PlayerFeature.InputFeature.Components",
            sourceClassName: "LockGameInputMarker/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : TagComponentTemplate<LockGameInputMarker>
        {
        }
    }
}