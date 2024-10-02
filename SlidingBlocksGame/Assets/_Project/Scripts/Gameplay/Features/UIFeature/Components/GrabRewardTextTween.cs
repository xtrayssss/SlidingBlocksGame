using System;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct GrabRewardTextTween : IEcsComponent
    {
        public Tween Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components", sourceClassName: "GrabRewardTextTween/Template", sourceAssembly: "Assembly-CSharp")]


		private sealed class Template : ComponentTemplate<GrabRewardTextTween>
        {
        }
    }
}