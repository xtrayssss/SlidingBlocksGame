using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.Components
{
    [Serializable]
    public struct CreationChainStrategyTag : IEcsTagComponent
    {
		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components", sourceClassName: "CreationChainTag/Template", sourceAssembly: "Assembly-CSharp")]
		private sealed class Template : TagComponentTemplate<CreationChainStrategyTag>
        {
        }
    }
}