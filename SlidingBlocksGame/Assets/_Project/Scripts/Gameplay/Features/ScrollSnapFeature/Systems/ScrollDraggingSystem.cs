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
            [Inc] public readonly EcsPool<ScrollSnap> ScrollSnaps;
            [Inc] public readonly EcsTagPool<DraggingState> DraggingStates;

            [Opt] public readonly EcsPool<ScrollToTargetState> ScrollToTargetState;
        }

        public void Run()
        {
            foreach (int scroll in _world.Where(out ScrollAspect scrollAspect))
            {
                ref ScrollSnap scrollSnap = ref scrollAspect.ScrollSnaps.Get(scroll);

                CheckIfLeavingItem(ref scrollSnap);
                
                if (!Input.GetMouseButton(0) && !IsSnapped(scrollSnap))
                {
                    scrollAspect.DraggingStates.Del(scroll);

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
                    int @event = _world.NewEntity();
                    _world.GetPool<LeaveItemEvent>().Add(@event).ItemIndex = scrollSnap.LastSnappedIndex;
                    scrollSnap.LastSnappedIndex = -1;
                }
            }
        }
    }
}