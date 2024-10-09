using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.DestructionFeature.Components
{
    [Serializable]
    public struct DeleteEntityRequest : IEcsTagComponent
    {
        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.DestructionFeature.Components",
            sourceClassName: "DeleteEntityRequest/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : TagComponentTemplate<DeleteEntityRequest>
        {
        }
    }
}