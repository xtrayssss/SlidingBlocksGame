using System;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.TweenFeature.Components
{
    [Serializable]
    [MetaGroup("Tween")]
    public struct TweenEase : IEcsComponent
    {
        public AnimationCurve Curve;
        public Ease Ease;

        private sealed class Template : ComponentTemplate<TweenEase>
        {
        }
    }
}