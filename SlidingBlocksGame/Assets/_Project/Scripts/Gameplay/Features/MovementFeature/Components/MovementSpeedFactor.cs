using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Components
{
    [Serializable]
    [MetaGroup("Movement")]
    public struct MovementSpeedFactor : IEcsComponent
    {
        public float Base;
        public float Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.MovementFeature.Components", sourceClassName: "MovementSpeedFactor/Template", sourceAssembly: "Assembly-CSharp")]
		private sealed class Template : ComponentTemplate<MovementSpeedFactor>
        {
        }
    }
}