using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Systems
{
    public class ScrollDraggingSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class ScrollAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(UnlockedMarker))]
            [Inc] public readonly EcsPool<ScrollSnap> ScrollSnaps;
            [Inc] public readonly EcsTagPool<DraggingState> DraggingState;
            [Opt] public readonly EcsPool<ScrollToTargetState> ScrollToTargetState;
            [Opt] public readonly EcsTagPool<ScrollNearestRequest> ScrollNearest;
        }

        private class ItemAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsTagPool<LeavedEvent> LeaveEvent;
            [Opt] public readonly EcsTagPool<LeaveMarker> LeaveMarker;
        }

        public void Run()
        {
            foreach (int scroll in _world.Where(out ScrollAspect scrollAspect))
            {
                ref ScrollSnap scrollSnap = ref scrollAspect.ScrollSnaps.Get(scroll);

                scrollAspect.ScrollNearest.TryAdd(scroll);
                CheckIfLeavingItem(ref scrollSnap);

                if (!Input.GetMouseButton(0) && !IsSnapped(scrollSnap))
                {
                    ItemAspect itemAspect = _world.GetAspect<ItemAspect>();

                    foreach (entlong item in scrollSnap.Items.Longs)
                    {
                        if (item.TryGetID(out int itemID))
                            itemAspect.LeaveMarker.TryDel(itemID);
                    }

                    scrollAspect.DraggingState.Del(scroll);
                    scrollAspect.ScrollToTargetState.Add(scroll);
                }
            }
        }

        private static bool IsSnapped(in ScrollSnap scrollSnap) =>
            Mathf.Abs(scrollSnap.Positions[scrollSnap.NearestIndex] -
                      scrollSnap.ScrollRect.horizontalScrollbar.value) <= scrollSnap.SnapDistanceThreshold;

        private void CheckIfLeavingItem(ref ScrollSnap scrollSnap)
        {
            if (scrollSnap.LastSnappedIndex != -1)
            {
                float itemPos = scrollSnap.Positions[scrollSnap.LastSnappedIndex];

                if (math.abs(scrollSnap.ScrollPosition - itemPos) > scrollSnap.SnapDistanceThreshold)
                {
                    ItemAspect itemAspect = _world.GetAspect<ItemAspect>();
                    int leaveItem = scrollSnap.Items[scrollSnap.LastSnappedIndex];
                    itemAspect.LeaveEvent.Add(leaveItem);
                    itemAspect.LeaveMarker.Add(leaveItem);
                    scrollSnap.LastSnappedIndex = -1;
                }
            }
        }
    }
}