using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Utils
{
    public static class ScrollSnapUtils
    {
        public static EcsSpan GetVisibles(in ScrollSnap scrollSnap)
        {
            int currentIndex = scrollSnap.NearestIndex;
            int maxVisibleHalf = (scrollSnap.MaxVisible - 1) / 2;

            int leftOffset = math.min(currentIndex, maxVisibleHalf);
            int rightOffset = math.min(scrollSnap.Items.Count - currentIndex - 1, maxVisibleHalf);

            int startIndex = currentIndex - leftOffset;
            int sliceLength = leftOffset + rightOffset + 1;

#if UNITY_EDITOR
            Debug.Assert(startIndex >= 0 && startIndex < scrollSnap.Items.Count, "Start index out of range");
#endif

            return scrollSnap.Items.Slice(startIndex, sliceLength);
        }
    }
}