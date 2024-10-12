using System;
using DCFApixels.DragonECS;
using UnityEngine.Serialization;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components
{
    [Serializable]
    [MetaGroup("ScrollSnap")]
    public struct EffectDisplacement : IEcsComponent
    {
        [FormerlySerializedAs("effectedDistanceBasedOnItemSize")] [FormerlySerializedAs("Value")] public float EffectedDistanceBasedOnItemSize;

        private sealed class Template : ComponentTemplate<EffectDisplacement>
        {
        }
    }
}