using System;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct OpenCloseTween : IEcsComponent
    {
        public Tween Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components", sourceClassName: "OpenCloseTween/Template", sourceAssembly: "Assembly-CSharp")]
		private sealed class Template : ComponentTemplate<OpenCloseTween>
        {
        }
    }
}