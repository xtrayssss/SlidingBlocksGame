using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Systems
{
    public class ScrollToTargetSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private Coroutine _startCoroutine;

        private class ScrollAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ScrollUnlockedMarker))]
            [Inc] public readonly EcsPool<ScrollSnap> ScrollSnaps;
            [Inc] public readonly EcsPool<ScrollToTargetState> ScrollToTargetStates;
            [Inc] public readonly EcsPool<GameObjectConnect> GoConnects;

            [Opt] public readonly EcsTagPool<DraggingState> DraggingStates;
            [Opt] public readonly EcsTagPool<IdleState> IdleState;
            [Opt] public readonly EcsTagPool<ScrollNearestRequest> ScrollNearest;
        }

        private class DelayAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CooldownExpiredEvent> CooldownExpiredEvent;
            [Inc] public readonly EcsTagPool<ScrollDelayTag> ScrollDelayTag;
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
                ref ScrollToTargetState scrollToTargetState = ref scrollAspect.ScrollToTargetStates.Get(scroll);
                ref GameObjectConnect goConnect = ref scrollAspect.GoConnects.Get(scroll);

                if (!scrollToTargetState.IsAutoScroll) 
                    scrollAspect.ScrollNearest.Add(scroll);
                else
                    scrollSnap.TargetPosition = scrollSnap.Positions[scrollSnap.TargetIndex];

                if (!scrollToTargetState.CurrentTween.isAlive && !scrollToTargetState.Delay.IsAlive)
                    StartScrollToTarget(
                        ref scrollSnap,
                        ref scrollToTargetState);

                if (Input.GetMouseButtonDown(0))
                {
                    scrollToTargetState.CurrentTween.Stop();
                    
                    if (scrollToTargetState.Delay.IsAlive)
                        _world.DelEntity(scrollToTargetState.Delay);

                    scrollAspect.ScrollToTargetStates.Del(scroll);
                    scrollAspect.DraggingStates.Add(scroll);
                }

                foreach (int _ in _world.Where(out DelayAspect _))
                {
                    scrollToTargetState.CurrentTween =
                        Sequence.Create()
                            .Chain(
                                Tween.UIHorizontalNormalizedPosition(
                                    target: scrollSnap.ScrollRect,
                                    endValue: scrollSnap.TargetPosition,
                                    duration: scrollSnap.SmoothScrollDuration,
                                    ease: scrollSnap.ScrollEase).OnComplete(
                                    target: goConnect.Connect,
                                    static connect =>
                                    {
                                        if (!connect.Entity.TryGetID(out int id))
                                            return;

                                        EcsWorld world = connect.World;
                                        ScrollAspect scrollAspect = world.GetAspect<ScrollAspect>();

                                        ref ScrollSnap scrollSnap = ref scrollAspect.ScrollSnaps.Get(id);
                                        scrollSnap.ScrollPosition = scrollSnap.TargetPosition;
                                        scrollSnap.LastSnappedIndex = scrollSnap.TargetIndex;

                                        int targetItem = scrollSnap.Items[scrollSnap.TargetIndex];

                                        if (!targetItem.ToEntityLong(world).TryGetID(out int targetItemID))
                                            return;

                                        ItemAspect itemAspect = world.GetAspect<ItemAspect>();
                                        itemAspect.SnappedEvent.Add(targetItemID);
                                        itemAspect.SnappedMarker.Add(targetItemID);

                                        scrollAspect.ScrollToTargetStates.Del(id);
                                        scrollAspect.IdleState.Add(id);
                                    }));
                }
            }
        }

        private void StartScrollToTarget(
            ref ScrollSnap scrollSnap,
            ref ScrollToTargetState scrollToTargetState)
        {
            int delay = _world.NewEntity();
            ref Cooldown cooldown = ref _world.GetPool<Cooldown>().Add(delay);
            cooldown.Duration = scrollSnap.SnapDelay;
            _world.GetPool<RefreshCooldownRequest>().Add(delay);
            _world.GetPool<ScrollDelayTag>().Add(delay);
            _world.GetPool<DeleteOnExpiredMarker>().Add(delay);

            scrollToTargetState.Delay = delay.ToEntityLong(_world);
        }
    }
}