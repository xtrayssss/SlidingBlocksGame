using System;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components
{
    [Serializable]
    [MetaGroup("ScrollSnap")]
    public struct ScrollSnap : IEcsComponent, IEcsComponentLifecycle<ScrollSnap>
    {
        [Header("Settings")]
        public float SnapDistanceThreshold;

        public ScrollRect ScrollRect;
        public float SmoothScrollDuration;
        public float SnapDelay;
        public Ease ScrollEase;

        [Header("Dynamic layout")]
        public RectTransform ElementTemplate;

        public HorizontalOrVerticalLayoutGroup LayoutGroup;
        public float VisiblePartRatio;
        public bool IsDebug;

        [Header("Effects")]
        public ScriptableEntityTemplate[] Effects;

        [Header("Runtime")]
        public float ScrollPosition;
        public float TargetPosition;
        public Sequence OpenCloseTween;
        public int TargetIndex;
        public int NearestIndex;
        public int ItemCount;
        public float[] Positions;
        public EcsGroup Items;
        public float Distance;
        public int LastSnappedIndex;
        public int MaxVisible;

        public void Enable(ref ScrollSnap component)
        {
            component.LastSnappedIndex = -1;
            component.TargetIndex = -1;
        }

        public void Disable(ref ScrollSnap component)
        {
            component.LastSnappedIndex = -1;
            component.TargetIndex = -1;
        }

        private sealed class Template : ComponentTemplate<ScrollSnap>
        {
        }
    }
}