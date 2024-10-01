using System;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Components
{
    [Serializable]
    [MetaGroup("Movement")]
    public struct MovementDirection : IEcsComponent
    {
        public int2 Value;

        [Serializable]

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.MovementFeature.Components", sourceClassName: "MovementDirection/Template", sourceAssembly: "Assembly-CSharp")]
		public sealed class Template : ComponentTemplate<MovementDirection>
        {
        }
    }
}