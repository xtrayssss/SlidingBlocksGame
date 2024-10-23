using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components
{
    [Serializable]
    [MetaGroup("ScrollSnap")]
    public struct EffectDisplacement : IEcsComponent
    {
        public float EffectedDistanceBasedOnItemSize;

        private sealed class Template : ComponentTemplate<EffectDisplacement>
        {
        }
    }
}