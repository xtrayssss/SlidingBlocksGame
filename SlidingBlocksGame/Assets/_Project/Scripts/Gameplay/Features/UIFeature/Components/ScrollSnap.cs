using System;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct ScrollSnap : IEcsComponent
    {
        public int NearestIndex;
        public float NearestPos;
        public EcsGroup Items;
        public float Distance;
        public float Position;
        public ScrollRect ScrollRect;
        public float SnapDuration;
        public Ease SnapEase;
        public Tween SnapTween;
        public ScriptableEntityTemplate[] EffectsConfigs;
        public EcsGroup Effects;
        public Sequence OpenCloseTween;
        public readonly EcsLongsSpan SafeItems => Items.Longs;

        private sealed class Template : ComponentTemplate<ScrollSnap>
        {
        }
    }
}