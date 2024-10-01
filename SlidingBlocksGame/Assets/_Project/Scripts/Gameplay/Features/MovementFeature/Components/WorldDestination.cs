using System;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Components
{
    [Serializable] 
    [MetaGroup("Movement")]
    public struct WorldDestination : IEcsComponent
    {
        public float3 Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.MovementFeature.Components", sourceClassName: "WorldDestination/Template", sourceAssembly: "Assembly-CSharp")]
		private sealed class Template : ComponentTemplate<WorldDestination>
        {
        }
    }
}