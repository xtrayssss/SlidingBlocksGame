using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.DestructionFeature.Components
{
    [Serializable]
    [MetaGroup("Destruction")]
    public struct DestructionChainStrategyTag : IEcsTagComponent
    {
        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.DestroyFeature.Components",
            sourceClassName: "DestructionChainTag/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : TagComponentTemplate<DestructionChainStrategyTag>
        {
        }
    }
}