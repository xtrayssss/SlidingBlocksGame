using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Components
{
    [Serializable]
    [MetaGroup("Movement")]
    public struct ChainMovementStrategyTag : IEcsTagComponent
    {
        [Serializable]

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.MovementFeature.Components", sourceClassName: "ChainMovementMarker/Wrapper", sourceAssembly: "Assembly-CSharp")]
		public sealed class Wrapper : TagComponentTemplate<ChainMovementStrategyTag>
        {
        }
    }
}