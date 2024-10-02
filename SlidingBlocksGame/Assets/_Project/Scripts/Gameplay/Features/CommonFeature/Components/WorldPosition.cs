using System;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct WorldPosition : IEcsComponent
    {
        public float3 Value;

        [Serializable]

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.CommonFeature.Components", sourceClassName: "WorldPosition/Wrapper", sourceAssembly: "Assembly-CSharp")]


		public sealed class Wrapper : ComponentTemplate<WorldPosition>
        {
        }
    }
}