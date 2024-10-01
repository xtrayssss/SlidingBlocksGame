using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.TweenFeature.Components
{
    [Serializable]
    [MetaGroup("Tween")]
    public struct TweenDuration : IEcsComponent
    {
        public float Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.TweenFeature.Components", sourceClassName: "TweenDuration/Template", sourceAssembly: "Assembly-CSharp")]
		private sealed class Template : ComponentTemplate<TweenDuration>
        {
        }
    }
}