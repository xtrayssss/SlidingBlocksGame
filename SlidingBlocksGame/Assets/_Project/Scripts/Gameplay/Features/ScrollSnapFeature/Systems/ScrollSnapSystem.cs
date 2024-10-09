using System;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Systems
{
    public class ScrollSnapSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class ScrollSnapAspect : EcsAspectAuto
        {
            [ExcImplicit(typeof(ScrollSetupRequest))]
            [Inc] public readonly EcsPool<ScrollSnap> ScrollSnaps;

            [Opt] public readonly EcsTagPool<ApplyEffectsMarker> ApplyEffects;

            [Opt] public readonly EcsTagPool<ScrollDraggedEvent> ScrollDraggedEvent;
            [Opt] public readonly EcsTagPool<ScrollSnappedEvent> ScrollSnappedEvent;
        }

        private class ScrollToTargetStateAspect : EcsAspectAuto
        {
            public class OnEnter : EcsAspectAuto
            {
                [IncImplicit(typeof(ScrollToTargetEvent))]
                [Inc] public readonly EcsPool<ScrollSnap> ScrollSnaps;

                [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;
            }

            public class OnUpdate : EcsAspectAuto
            {
                [Inc] public readonly EcsPool<ScrollSnap> ScrollSnaps;
                [Inc] public readonly EcsTagPool<ScrollToTargetMarker> ScrollToTargetMarker;

                [Opt] public readonly EcsTagPool<ScrollDraggedEvent> ScrollDraggedEvent;
                [Opt] public readonly EcsTagPool<ScrollDraggedMarker> ScrollDraggedMarker;
                [Opt] public readonly EcsTagPool<ScrollToTargetEvent> ScrollToTargetEvent;
            }
        }

        private class ScrollSnappedStateAspect : EcsAspectAuto
        {
            public class OnEnter : EcsAspectAuto
            {
                [Inc] public readonly EcsTagPool<ScrollSnappedEvent> ScrollSnappedEvent;
                [Inc] public readonly EcsPool<ScrollSnap> ScrollSnaps;
                [Opt] public readonly EcsTagPool<ScrollSnappedMarker> ScrollSnappedMarker;
            }

            public class OnUpdate : EcsAspectAuto
            {
                [Inc] public readonly EcsPool<ScrollSnap> ScrollSnaps;
                [Inc] public readonly EcsTagPool<ScrollSnappedMarker> ScrollSnappedMarker;

                [Opt] public readonly EcsTagPool<ScrollDraggedEvent> ScrollDraggedEvent;
                [Opt] public readonly EcsTagPool<ScrollDraggedMarker> ScrollDraggedMarker;
                [Opt] public readonly EcsTagPool<ScrollSnappedEvent> ScrollSnappedEvent;
            }
        }

        private class ScrollDragStateAspect : EcsAspectAuto
        {
            public class OnEnter : EcsAspectAuto
            {
                [Inc] public readonly EcsTagPool<ScrollDraggedEvent> ScrollDraggedEvent;
                [Inc] public readonly EcsPool<ScrollSnap> ScrollSnaps;
                [Opt] public readonly EcsTagPool<ScrollDraggedMarker> ScrollDraggedMarker;
            }

            public class OnUpdate : EcsAspectAuto
            {
                [Inc] public readonly EcsTagPool<ScrollDraggedMarker> ScrollDraggedMarker;
                [Inc] public readonly EcsPool<ScrollSnap> ScrollSnaps;

                [Opt] public readonly EcsTagPool<ScrollToTargetEvent> ScrollToTargetEvent;
                [Opt] public readonly EcsTagPool<ScrollToTargetMarker> ScrollToTargetMarker;
                [Opt] public readonly EcsTagPool<ScrollDraggedEvent> ScrollDraggedEvent;
            }
        }

        private class SetupStateAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<ScrollSnap> ScrollSnaps;
            [Inc] public readonly EcsPool<ScrollSetupRequest> ScrollSetupRequest;
        }

        private class ScaleEffectAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ScaleEffectTag))]
            [IncImplicit(typeof(ApplyEffectRequest))]
            [Inc] public readonly EcsPool<ScrollSnapEffect> ScrollSnapEffects;

            [Inc] public readonly EcsPool<SelectionStateEffectFactor> SelectionStateEffectFactors;
            [Inc] public readonly EcsPool<TargetEntity> Elements;
        }

        private class FadeEffectAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(FadeEffectTag))]
            [IncImplicit(typeof(ApplyEffectRequest))]
            [Inc] public readonly EcsPool<ScrollSnapEffect> ScrollSnapEffects;

            [Inc] public readonly EcsPool<TargetEntity> Elements;
            [Inc] public readonly EcsPool<FadeAlphaEffectFactor> FadeAlphaEffectFactors;
        }

        private class UnlockAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(UnlockScrollSnapRequest))]
            [Inc] public readonly EcsPool<ScrollSnap> ScrollSnaps;

            [Opt] public readonly EcsTagPool<ApplyEffectsMarker> ApplyEffectsMarker;
        }

        private class LockStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LockScrollSnapRequest))]
            [Inc] public readonly EcsPool<ScrollSnap> ScrollSnaps;

            [Inc] public readonly EcsTagPool<ApplyEffectsMarker> ApplyEffectsMarker;
        }

        private class ElementAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<ScrollElement> ScrollElements;
        }

        private void UpdateNearest(ref ScrollSnap scrollSnap)
        {
            int nearest = GetNearestIndex(in scrollSnap);

            if (nearest != -1)
                scrollSnap.NearestIndex = nearest;

            ElementAspect elementAspect = _world.GetAspect<ElementAspect>();

            if (scrollSnap.SafeItems[scrollSnap.NearestIndex].TryGetID(out int elementID))
            {
                ref readonly ScrollElement scrollElement = ref elementAspect.ScrollElements.Read(elementID);

                scrollSnap.NearestPosition = scrollElement.Position;
            }
        }

        private int GetNearestIndex(in ScrollSnap scrollSnap)
        {
            if (scrollSnap.Items.Count <= 1)
                return 0;

            ElementAspect elementAspect = _world.GetAspect<ElementAspect>();

            for (int i = 0; i < scrollSnap.SafeItems.Count; i++)
            {
                if (!scrollSnap.SafeItems[i].TryGetID(out int elementID))
                    continue;

                ref readonly ScrollElement scrollElement = ref elementAspect.ScrollElements.Read(elementID);

                if (math.abs(scrollSnap.Position - scrollElement.Position) <= scrollSnap.Distance / 2)
                    return i;
            }

            return -1;
        }

        public void Run()
        {
            foreach (int scroll in _world.Where(out UnlockAspect unlockAspect))
            {
                ref ScrollSnap scrollSnap = ref unlockAspect.ScrollSnaps.Get(scroll);

                scrollSnap.ScrollRect.enabled = true;

                unlockAspect.ApplyEffectsMarker.Add(scroll);

                Debug.Log(scrollSnap.TargetIndex);
                Debug.Log(scrollSnap.NearestIndex);

                ElementAspect elementAspect = _world.GetAspect<ElementAspect>();

                if (scrollSnap.TargetIndex != scrollSnap.NearestIndex)
                {
                    if (scrollSnap.SafeItems[scrollSnap.TargetIndex].TryGetID(out int elementID))
                    {
                        ref ScrollElement scrollElement = ref elementAspect.ScrollElements.Get(elementID);

                        scrollSnap.TargetPosition = scrollElement.Position;
                    }

                    _world.GetPool<ScrollToTargetEvent>().Add(scroll);
                    _world.GetPool<ScrollToTargetMarker>().Add(scroll);
                }
                else
                {
                    _world.GetPool<ScrollSnappedEvent>().Add(scroll);
                    _world.GetPool<ScrollSnappedMarker>().Add(scroll);
                }

                Debug.Log("SCROLL_UNLOCKED");
            }

            foreach (int entity in _world.Where(out LockStateAspect lockStateAspect))
            {
                ref ScrollSnap scrollSnap = ref lockStateAspect.ScrollSnaps.Get(entity);

                scrollSnap.ScrollRect.horizontalScrollbar.value = scrollSnap.TargetPosition;

                scrollSnap.SnapTween.Stop();

                lockStateAspect.ApplyEffectsMarker.Del(entity);

                scrollSnap.ScrollRect.enabled = false;

                _world.GetPool<ScrollToTargetMarker>().TryDel(entity);
                _world.GetPool<ScrollSnappedMarker>().TryDel(entity);
                _world.GetPool<ScrollDraggedMarker>().TryDel(entity);

                if (scrollSnap.Selected.TryGetID(out int selectedID))
                    _world.GetPool<ScrollSnappedMarker>().TryDel(selectedID);

                Debug.Log("SCROLL_LOCKED");
            }

            foreach (int scroll in _world.Where(out SetupStateAspect aspect))
            {
                ref ScrollSnap scrollSnap = ref aspect.ScrollSnaps.Get(scroll);

                ref readonly ScrollSetupRequest scrollSetupRequest = ref aspect.ScrollSetupRequest.Read(scroll);

                int count = scrollSnap.SafeItems.Count;

                ElementAspect elementAspect = _world.GetAspect<ElementAspect>();


                scrollSnap.Distance = count > 1 ? 1f / (count - 1f) : 1;

                for (int i = 0; i < count; i++)
                {
                    if (scrollSnap.SafeItems[i].TryGetID(out int elementID))
                    {
                        ref ScrollElement scrollElement = ref elementAspect.ScrollElements.Get(elementID);
                        scrollElement.Position = scrollSnap.Distance * i;
                    }
                }

                scrollSnap.Effects = EcsGroup.New(_world);

                for (int index = 0; index < scrollSnap.SafeItems.Count; index++)
                {
                    if (!scrollSnap.SafeItems[index].TryGetID(out int elementID))
                        continue;

                    ref ScrollElement scrollElement = ref elementAspect.ScrollElements.Get(elementID);

                    foreach (ScriptableEntityTemplate effectCfg in scrollSnap.EffectsConfigs)
                    {
                        int effect = _world.NewEntity(effectCfg);

                        scrollSnap.Effects.Add(effect);

                        float signedDist = (scrollElement.Position - scrollSnap.Position) / (scrollSnap.Distance * 1);

                        float displacement = Mathf.Clamp(signedDist, -1, 1);

                        _world.GetPool<ScrollSnapEffect>().Add(effect).Displacement = displacement;
                    }
                }

                scrollSnap.TargetIndex = scrollSetupRequest.TargetIndex;
            }

            foreach (int entity in _world.Where(out ScrollSnapAspect aspect))
            {
                ref ScrollSnap scrollSnap = ref aspect.ScrollSnaps.Get(entity);

                scrollSnap.Position = scrollSnap.ScrollRect.horizontalScrollbar.value;

                if (Input.GetMouseButtonDown(0))
                {
                    scrollSnap.LastScrollPosition = scrollSnap.Position;
                }

                UpdateNearest(ref scrollSnap);

                if (scrollSnap.Selected.TryGetID(out int selectedID))
                {
                    aspect.ScrollDraggedEvent.TryDel(selectedID);
                    aspect.ScrollSnappedEvent.TryDel(selectedID);
                }

                if (aspect.ApplyEffects.Has(entity))
                {
                    for (int index = 0; index < scrollSnap.SafeItems.Count; index++)
                    {
                        if (!scrollSnap.SafeItems[index].TryGetID(out int elementID))
                            continue;

                        ElementAspect elementAspect = _world.GetAspect<ElementAspect>();

                        ref ScrollElement scrollElement = ref elementAspect.ScrollElements.Get(elementID);

                        float signedDist = (scrollElement.Position - scrollSnap.Position) / (scrollSnap.Distance * 1);

                        float displacement = Mathf.Clamp(signedDist, -1, 1);

                        foreach (ScriptableEntityTemplate effectCfg in scrollSnap.EffectsConfigs)
                        {
                            int effect = _world.NewEntity(effectCfg);

                            scrollSnap.Effects.Add(effect);

                            _world.GetPool<ScrollSnapEffect>().Add(effect).Displacement = displacement;
                            _world.GetPool<ApplyEffectRequest>().Add(effect);
                            _world.GetPool<TargetEntity>().Add(effect).Value = scrollSnap.SafeItems[index];
                        }
                    }
                }
            }


            foreach (int entity in _world.Where(out ScrollSnappedStateAspect.OnEnter aspect))
            {
                ref ScrollSnap scrollSnap = ref aspect.ScrollSnaps.Get(entity);

                scrollSnap.Selected = scrollSnap.SafeItems[scrollSnap.TargetIndex];

                if (scrollSnap.Selected.TryGetID(out int selectedID))
                {
                    aspect.ScrollSnappedEvent.Add(selectedID);
                    aspect.ScrollSnappedMarker.Add(selectedID);
                }
            }

            foreach (int entity in _world.Where(out ScrollSnappedStateAspect.OnUpdate aspect))
            {
                aspect.ScrollSnappedEvent.TryDel(entity);

                ref ScrollSnap scrollSnap = ref aspect.ScrollSnaps.Get(entity);

                if (Input.GetMouseButton(0) && Math.Abs(scrollSnap.Position - scrollSnap.LastScrollPosition) > 0.01f)
                {
                    if (scrollSnap.Selected.TryGetID(out int selectedID))
                        aspect.ScrollSnappedMarker.Del(selectedID);

                    aspect.ScrollDraggedEvent.Add(entity);
                    aspect.ScrollDraggedMarker.Add(entity);

                    aspect.ScrollSnappedMarker.Del(entity);
                }
            }

            foreach (int entity in _world.Where(out ScrollDragStateAspect.OnEnter aspect))
            {
                ref ScrollSnap scrollSnap = ref aspect.ScrollSnaps.Get(entity);

                scrollSnap.SnapTween.Stop();

                if (scrollSnap.Selected.TryGetID(out int selectedID))
                    aspect.ScrollDraggedEvent.Add(selectedID);
            }

            foreach (int entity in _world.Where(out ScrollDragStateAspect.OnUpdate aspect))
            {
                aspect.ScrollDraggedEvent.TryDel(entity);

                if (Input.GetMouseButtonUp(0))
                {
                    aspect.ScrollDraggedMarker.Del(entity);

                    aspect.ScrollToTargetEvent.Add(entity);
                    aspect.ScrollToTargetMarker.Add(entity);

                    ref ScrollSnap scrollSnap = ref aspect.ScrollSnaps.Get(entity);

                    scrollSnap.TargetPosition = scrollSnap.NearestPosition;
                    scrollSnap.TargetIndex = scrollSnap.NearestIndex;
                }
            }

            foreach (int entity in _world.Where(out ScrollToTargetStateAspect.OnEnter aspect))
            {
                ref ScrollSnap scrollSnap = ref aspect.ScrollSnaps.Get(entity);

                ref GameObjectConnect gameObjectConnect = ref aspect.GameObjectConnects.Get(entity);

                Debug.Log("SnapTween");

                scrollSnap.SnapTween = Tween
                    .UIHorizontalNormalizedPosition(
                        target: scrollSnap.ScrollRect,
                        endValue: scrollSnap.TargetPosition,
                        duration: scrollSnap.SnapDuration,
                        ease: scrollSnap.SnapEase)
                    .OnComplete(
                        target: gameObjectConnect.Connect,
                        onComplete: static connect =>
                        {
                            if (!connect.Entity.TryGetID(out int id))
                                return;

                            Debug.Log("SCROLL SNAPPED");

                            EcsWorld world = connect.World;

                            world.GetPool<ScrollSnappedMarker>().Add(id);
                            world.GetPool<ScrollSnappedEvent>().Add(id);

                            world.GetPool<ScrollToTargetMarker>().Del(id);
                        });
            }

            foreach (int entity in _world.Where(out ScrollToTargetStateAspect.OnUpdate aspect))
            {
                ref ScrollSnap scrollSnap = ref aspect.ScrollSnaps.Get(entity);

                aspect.ScrollToTargetEvent.TryDel(entity);

                if (Input.GetMouseButton(0) && Math.Abs(scrollSnap.Position - scrollSnap.LastScrollPosition) > 0.01f)
                {
                    Debug.Log("NEXT");
                    aspect.ScrollDraggedEvent.Add(entity);
                    aspect.ScrollDraggedMarker.Add(entity);

                    aspect.ScrollToTargetMarker.Del(entity);
                }
            }

            foreach (int effectID in _world.Where(out ScaleEffectAspect effectAspect))
            {
                ref readonly ScrollSnapEffect effect = ref effectAspect.ScrollSnapEffects.Read(effectID);

                ref readonly SelectionStateEffectFactor selectionFactor =
                    ref effectAspect.SelectionStateEffectFactors.Read(effectID);

                if (!effectAspect.Elements.Read(effectID).Value.TryGetID(out int elementID))
                    continue;

                ElementAspect elementAspect = _world.GetAspect<ElementAspect>();

                ref ScrollElement scrollElement = ref elementAspect.ScrollElements.Get(elementID);

                float ratio = 1 - math.abs(effect.Displacement);

                float3 targetScale = selectionFactor.Deselected +
                                     (selectionFactor.Selected - selectionFactor.Deselected) * ratio;

                scrollElement.RectTransform.localScale = math.lerp(
                    start: scrollElement.RectTransform.localScale,
                    end: targetScale,
                    t: 0.04f);

                _world.DelEntity(effectID);
            }

            foreach (int effectID in _world.Where(out FadeEffectAspect aspect))
            {
                ref readonly ScrollSnapEffect effect = ref aspect.ScrollSnapEffects.Read(effectID);

                ref readonly FadeAlphaEffectFactor fadeFactor = ref aspect.FadeAlphaEffectFactors.Read(effectID);

                if (!aspect.Elements.Read(effectID).Value.TryGetID(out int elementID))
                    continue;

                float ratio = 1 - math.abs(effect.Displacement);

                float targetAlpha = fadeFactor.Value + (1 - fadeFactor.Value) * ratio;

                ElementAspect elementAspect = _world.GetAspect<ElementAspect>();

                ref ScrollElement scrollElement = ref elementAspect.ScrollElements.Get(elementID);

                scrollElement.Graphic.color = new Color
                {
                    r = scrollElement.Graphic.color.r,
                    g = scrollElement.Graphic.color.g,
                    b = scrollElement.Graphic.color.g,
                    a = targetAlpha
                };

                _world.DelEntity(effectID);
            }
        }
    }
}