using System;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.TweenFeature.Components
{
    [Serializable]
    [MetaGroup("Tween")]
    public struct TweenEase : IEcsComponent
    {
        public AnimationCurve Curve;
        public Ease Ease;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.TweenFeature.Components", sourceClassName: "TweenEase/Template", sourceAssembly: "Assembly-CSharp")]


		private sealed class Template : ComponentTemplate<TweenEase>
        {
        }
    }
}