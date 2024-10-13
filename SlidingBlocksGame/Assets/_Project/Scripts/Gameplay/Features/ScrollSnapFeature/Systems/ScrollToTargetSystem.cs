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
            [Inc] public readonly EcsPool<ScrollSnap> ScrollSnaps;
            [Inc] public readonly EcsPool<ScrollToTargetState> ScrollToTargetStates;
            [Inc] public readonly EcsPool<GameObjectConnect> GoConnects;

            [Opt] public readonly EcsTagPool<DraggingState> DraggingStates;
        }

        private class DelayAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CooldownExpiredEvent> CooldownExpiredEvent;
            [Inc] public readonly EcsTagPool<ScrollDelayTag> ScrollDelayTag;
        }

        public void Run()
        {
            foreach (int scroll in _world.Where(out ScrollAspect scrollAspect))
            {
                ref ScrollSnap scrollSnap = ref scrollAspect.ScrollSnaps.Get(scroll);
                ref ScrollToTargetState scrollToTargetState = ref scrollAspect.ScrollToTargetStates.Get(scroll);
                ref GameObjectConnect goConnect = ref scrollAspect.GoConnects.Get(scroll);

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
                    //scrollSnap.ScrollRect.StopMovement();
                    
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

                                        int @event = world.NewEntity();

                                        world.GetPool<SnapToItemEvent>().Add(@event).ItemIndex =
                                            scrollSnap.TargetIndex;

                                        world.GetPool<ScrollToTargetState>().Del(id);
                                        world.GetPool<IdleState>().Add(id);
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