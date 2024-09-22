using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.TweenFeature.Components
{
    [Serializable]
    [MetaGroup("Tween")]
    public struct TweenStartDelay : IEcsComponent
    {
        public float Value;

        private sealed class Template : ComponentTemplate<TweenStartDelay>
        {
        }
    }
}