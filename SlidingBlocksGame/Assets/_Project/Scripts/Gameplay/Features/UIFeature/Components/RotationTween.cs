using System;
using DCFApixels.DragonECS;
using PrimeTween;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct RotationTween : IEcsComponent
    {
        public float Duration;
        public Tween Tween;
        
        private sealed class Template : ComponentTemplate<RotationTween>
        {
        }
    }
}