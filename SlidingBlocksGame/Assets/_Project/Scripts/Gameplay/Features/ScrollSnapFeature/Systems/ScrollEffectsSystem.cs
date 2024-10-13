using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Systems
{
    public class ScrollEffectsSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class ScaleEffectAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<ApplyEffectRequest> ApplyEffectRequest;
            [Inc] public readonly EcsPool<ScaleEffect> ScaleEffects;
        }

        private class FadeEffectAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<ApplyEffectRequest> ApplyEffectRequest;
            [Inc] public readonly EcsPool<FadeEffect> FadeEffects;
        }

        private class ItemAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<ScrollItem> ScrollItems;
        }

        public void Run()
        {
            foreach (int effect in _world.Where(out ScaleEffectAspect effectAspect))
            {
                ref readonly ApplyEffectRequest effectRequest = ref effectAspect.ApplyEffectRequest.Read(effect);

                ref ScaleEffect scaleEffect = ref effectAspect.ScaleEffects.Get(effect);

                ItemAspect itemAspect = _world.GetAspect<ItemAspect>();

                foreach (int item in effectRequest.Items)
                {
                    Vector2 diff = scaleEffect.SelectedItemScale - scaleEffect.UnselectedItemScale;
                    ref ScrollItem scrollItem = ref itemAspect.ScrollItems.Get(item);
                    scrollItem.RectTransform.localScale = scaleEffect.UnselectedItemScale + diff * effectRequest.Ratio;
                }
            }
            
            foreach (int effect in _world.Where(out FadeEffectAspect effectAspect))
            {
                Debug.Log("FADE");
                ref readonly ApplyEffectRequest effectRequest = ref effectAspect.ApplyEffectRequest.Read(effect);

                ref FadeEffect fadeEffect = ref effectAspect.FadeEffects.Get(effect);

                ItemAspect itemAspect = _world.GetAspect<ItemAspect>();

                foreach (int item in effectRequest.Items)
                {
                    float targetAlpha = fadeEffect.FadeAlpha + (1 - fadeEffect.FadeAlpha) * effectRequest.Ratio;
                    ref ScrollItem scrollItem = ref itemAspect.ScrollItems.Get(item);

                    scrollItem.Graphic.color = new Color
                    {
                        r = scrollItem.Graphic.color.r,
                        g = scrollItem.Graphic.color.g,
                        b = scrollItem.Graphic.color.b,
                        a = targetAlpha
                    };
                }
            }
        }
    }
}