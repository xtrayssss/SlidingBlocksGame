using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CreationFeature.Components
{
    [Serializable]
    [MetaGroup("Creation")]
    public struct CreationChainStrategyTag : IEcsTagComponent
    {
        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.GameFlowFeature.Components",
            sourceClassName: "CreationChainStrategyTag/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : TagComponentTemplate<CreationChainStrategyTag>
        {
        }
    }
}