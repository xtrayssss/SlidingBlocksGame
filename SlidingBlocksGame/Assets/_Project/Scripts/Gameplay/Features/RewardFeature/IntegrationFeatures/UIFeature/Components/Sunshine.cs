using System;
using DCFApixels.DragonECS;
using PrimeTween;

namespace _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.UIFeature.Components
{
    [Serializable]
    [MetaGroup("Reward/UI")]
    public struct Sunshine : IEcsComponent
    {
        public Tween RotationTween;

        private sealed class Template : ComponentTemplate<Sunshine>
        {
        }
    }
}