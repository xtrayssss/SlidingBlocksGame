using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.DestroyFeature.Components
{
    [Serializable]
    public struct DestructionStrategyTag : IEcsTagComponent
    {

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.DestroyFeature.Components", sourceClassName: "DestructionStrategyTag/Template", sourceAssembly: "Assembly-CSharp")]
		private sealed class Template : TagComponentTemplate<DestructionStrategyTag>
        {
        }
    }
}