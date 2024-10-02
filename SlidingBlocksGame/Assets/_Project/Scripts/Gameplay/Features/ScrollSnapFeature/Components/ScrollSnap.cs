using System;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components
{
    [Serializable]
    public struct ScrollSnap : IEcsComponent
    {
        public int TargetIndex;
        public float TargetPosition;
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
        public float LastScrollPosition;
        public entlong Selected;
        public int NearestIndex;
        public float NearestPosition;
        public readonly EcsLongsSpan SafeItems => Items.Longs;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components", sourceClassName: "ScrollSnap/Template", sourceAssembly: "Assembly-CSharp")]


		private sealed class Template : ComponentTemplate<ScrollSnap>
        {
        }
    }
}