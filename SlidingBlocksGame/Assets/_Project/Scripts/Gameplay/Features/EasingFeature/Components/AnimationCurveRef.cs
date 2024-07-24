using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.EasingFeature.Components
{
    [Serializable]
    public struct AnimationCurveRef : IEcsComponent
    {
        public AnimationCurve Value;

        public sealed class Wrapper : ComponentTemplate<AnimationCurveRef>
        {
        }
    }
}