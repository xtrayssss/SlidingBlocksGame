using System;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class ScrollSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class ScrollSnapAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<ScrollSnap> ScrollSnaps;
            [Opt] public readonly EcsTagPool<ScrollSnappedEvent> ScrollSnappedEvent;
            [Opt] public readonly EcsTagPool<ScrollStartedEvent> ScrollStartedEvent;
            [Opt] public readonly EcsTagPool<ApplyEffectsMarker> ApplyEffects;
        }

        private class SnapStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ScrollSnappedEvent))]
            [Inc] public readonly EcsPool<ScrollSnap> ScrollSnaps;
        }

        private class ScrollStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ScrollStartedEvent))]
            [Inc] public readonly EcsPool<ScrollSnap> ScrollSnaps;
        }

        private class SetupStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ScrollSetupRequest))]
            [Inc] public readonly EcsPool<ScrollSnap> ScrollSnaps;
        }

        private class ScaleEffectAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ScaleEffectTag))]
            [IncImplicit(typeof(ApplyEffectRequest))]
            [Inc] public readonly EcsPool<ScrollSnapEffect> ScrollSnapEffects;

            [Inc] public readonly EcsPool<SelectionStateEffectFactor> SelectionStateEffectFactors;
            [Inc] public readonly EcsPool<TargetEntity> Targets;
        }


        private class FadeEffectAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(FadeEffectTag))]
            [IncImplicit(typeof(ApplyEffectRequest))]
            [Inc] public readonly EcsPool<ScrollSnapEffect> ScrollSnapEffects;

            [Inc] public readonly EcsPool<TargetEntity> Targets;
            [Inc] public readonly EcsPool<FadeAlphaEffectFactor> FadeAlphaEffectFactors;
        }

        private void UpdateNearest(ref ScrollSnap scrollSnap)
        {
            int nearest = GetNearestIndex(in scrollSnap);

            if (nearest != -1)
                scrollSnap.NearestIndex = nearest;

            if (scrollSnap.SafeItems[scrollSnap.NearestIndex].TryGetID(out int itemID))
                scrollSnap.NearestPos = _world.GetPool<ScrollPosition>().Read(itemID).Value;
        }


        private int GetNearestIndex(in ScrollSnap scrollSnap)
        {
            if (scrollSnap.Items.Count <= 1)
                return 0;

            for (int i = 0; i < scrollSnap.SafeItems.Count; i++)
            {
                if (!scrollSnap.SafeItems[i].TryGetID(out int itemID))
                    continue;

                float position = _world.GetPool<ScrollPosition>().Read(itemID).Value;

                if (Math.Abs(scrollSnap.Position - position) <= scrollSnap.Distance / 2)
                    return i;
            }

            return -1;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out SetupStateAspect aspect))
            {
                ref ScrollSnap scrollSnap = ref aspect.ScrollSnaps.Get(entity);

                int count = scrollSnap.SafeItems.Count;

                scrollSnap.Distance = count > 1 ? 1f / (count - 1f) : 1;

                for (int i = 0; i < count; i++)
                {
                    if (scrollSnap.SafeItems[i].TryGetID(out int itemID))
                        _world.GetPool<ScrollPosition>().Get(itemID).Value = scrollSnap.Distance * i;
                }

                scrollSnap.Effects = EcsGroup.New(_world);

                for (int index = 0; index < scrollSnap.SafeItems.Count; index++)
                {
                    if (!scrollSnap.SafeItems[index].TryGetID(out int itemID))
                        continue;

                    float itemPosition = _world.GetPool<ScrollPosition>().Read(itemID).Value;

                    foreach (ScriptableEntityTemplate effectCfg in scrollSnap.EffectsConfigs)
                    {
                        int effect = _world.NewEntity(effectCfg);

                        scrollSnap.Effects.Add(effect);

                        float signedDist = (itemPosition - scrollSnap.Position) / (scrollSnap.Distance * 1);

                        float displacement = Mathf.Clamp(signedDist, -1, 1);

                        _world.GetPool<ScrollSnapEffect>().Add(effect).Displacement = displacement;
                    }
                }
            }

            foreach (int entity in _world.Where(out ScrollSnapAspect aspect))
            {
                ref ScrollSnap scrollSnap = ref aspect.ScrollSnaps.Get(entity);

                scrollSnap.Position = scrollSnap.ScrollRect.horizontalScrollbar.value;

                UpdateNearest(ref scrollSnap);

                if (Input.GetMouseButtonDown(0))
                    aspect.ScrollStartedEvent.Add(entity);

                if (Input.GetMouseButtonUp(0))
                    aspect.ScrollSnappedEvent.Add(entity);

                if (aspect.ApplyEffects.Has(entity))
                {
                    for (int index = 0; index < scrollSnap.SafeItems.Count; index++)
                    {
                        if (!scrollSnap.SafeItems[index].TryGetID(out int itemID))
                            continue;

                        float itemPosition = _world.GetPool<ScrollPosition>().Read(itemID).Value;

                        float signedDist = (itemPosition - scrollSnap.Position) / (scrollSnap.Distance * 1);

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

            foreach (int entity in _world.Where(out ScaleEffectAspect aspect))
            {
                ref readonly ScrollSnapEffect effect = ref aspect.ScrollSnapEffects.Read(entity);

                ref readonly SelectionStateEffectFactor selectionFactor =
                    ref aspect.SelectionStateEffectFactors.Read(entity);

                if (!aspect.Targets.Read(entity).Value.TryGetID(out int itemID))
                    continue;

                RectTransformRef transform = _world.GetPool<RectTransformRef>().Get(itemID);

                float ratio = 1 - math.abs(effect.Displacement);

                float3 targetScale = selectionFactor.Deselected +
                                     (selectionFactor.Selected - selectionFactor.Deselected) * ratio;

                transform.Value.localScale =
                    math.lerp(transform.Value.localScale, targetScale, 0.04f);

                _world.GetPool<DeleteEntityCommand>().Add(entity);
            }

            foreach (int entity in _world.Where(out FadeEffectAspect aspect))
            {
                ref readonly ScrollSnapEffect effect = ref aspect.ScrollSnapEffects.Read(entity);

                ref readonly FadeAlphaEffectFactor fadeFactor = ref aspect.FadeAlphaEffectFactors.Read(entity);

                if (!aspect.Targets.Read(entity).Value.TryGetID(out int targetID))
                    continue;

                float ratio = 1 - math.abs(effect.Displacement);

                var targetAlpha = fadeFactor.Value + (1 - fadeFactor.Value) * ratio;

                ref GraphicRef graphic = ref _world.GetPool<GraphicRef>().Get(targetID);
                graphic.Value.color = new Color(graphic.Value.color.r, graphic.Value.color.g, graphic.Value.color.b,
                    targetAlpha);

                _world.GetPool<DeleteEntityCommand>().Add(entity);
            }

            foreach (int entity in _world.Where(out ScrollStateAspect aspect))
            {
                ref ScrollSnap scrollSnap = ref aspect.ScrollSnaps.Get(entity);

                scrollSnap.SnapTween.Stop();

                if (scrollSnap.SafeItems[scrollSnap.NearestIndex].TryGetID(out int itemID))
                {
                    _world.GetPool<SnappedMarker>().TryDel(itemID);
                    _world.GetPool<ScrollSnappedEvent>().TryDel(itemID);

                    _world.GetPool<ScrollStartedEvent>().Add(itemID);
                    _world.GetPool<ScrollStartedMarker>().Add(itemID);
                }
            }

            foreach (int entity in _world.Where(out SnapStateAspect aspect))
            {
                ref ScrollSnap scrollSnap = ref aspect.ScrollSnaps.Get(entity);

                scrollSnap.SnapTween = Tween.UIHorizontalNormalizedPosition(scrollSnap.ScrollRect,
                    scrollSnap.NearestPos, scrollSnap.SnapDuration, scrollSnap.SnapEase);

                if (scrollSnap.SafeItems[scrollSnap.NearestIndex].TryGetID(out int itemID))
                {
                    _world.GetPool<SnappedMarker>().Add(itemID);
                    _world.GetPool<ScrollSnappedEvent>().Add(itemID);

                    _world.GetPool<ScrollStartedEvent>().TryDel(itemID);
                    _world.GetPool<ScrollStartedMarker>().TryDel(itemID);
                }
            }
        }
    }
}