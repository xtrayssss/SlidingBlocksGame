using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Systems
{
    namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Systems
    {
        public class ScrollIdleSystem : IEcsRun
        {
            [EcsInject] private EcsDefaultWorld _world;

            private class ScrollAspect : EcsAspectAuto
            {
                [IncImplicit(typeof(ScrollUnlockedMarker))]
                [Inc] public readonly EcsTagPool<IdleState> IdleStates;
                [Inc] public readonly EcsPool<ScrollSnap> ScrollSnaps;
                [Opt] public readonly EcsTagPool<DraggingState> DraggingState;
                [Opt] public readonly EcsTagPool<ScrollNearestRequest> ScrollNearest;
            }

            private class ItemAspect : EcsAspectAuto
            {
                [Inc] public readonly EcsTagPool<SnappedEvent> SnappedEvent;
                [Inc] public readonly EcsTagPool<SnappedMarker> SnappedMarker;
            }

            public void Run()
            {
                foreach (int scroll in _world.Where(out ScrollAspect scrollAspect))
                {
                    ref ScrollSnap scrollSnap = ref scrollAspect.ScrollSnaps.Get(scroll);
                    
                    float itemPos = scrollSnap.Positions[scrollSnap.TargetIndex];
                    
                    scrollAspect.ScrollNearest.TryAdd(scroll);
                    
                    if (Input.GetMouseButton(0) && math.abs(scrollSnap.ScrollPosition - itemPos) >
                        scrollSnap.SnapDistanceThreshold)
                    {
                        ItemAspect itemAspect = _world.GetAspect<ItemAspect>();

                        foreach (entlong item in scrollSnap.Items.Longs)
                        {
                            if (item.TryGetID(out int itemID))
                                itemAspect.SnappedMarker.TryDel(itemID);
                        }

                        scrollAspect.IdleStates.Del(scroll);
                        scrollAspect.DraggingState.Add(scroll);
                    }
                }
            }
        }
    }
}