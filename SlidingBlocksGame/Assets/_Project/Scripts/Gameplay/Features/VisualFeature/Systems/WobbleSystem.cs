using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.Systems
{
    public class WobbleSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(WobbleRequest))]
            [Inc] public readonly EcsPool<RectTransformRef> RectTransforms;

            [Opt] public readonly EcsPool<WobbleTween> WobbleTween;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref RectTransformRef rect = ref aspect.RectTransforms.Get(entity);

                aspect.WobbleTween.TryAddOrGet(entity).Value = Tween.UIAnchoredPosition(
                    target: rect.Value,
                    endValue: rect.Value.anchoredPosition + new Vector2(0, 45),
                    duration: 1.3f,
                    cycles: -1,
                    cycleMode: CycleMode.Yoyo,
                    ease: Ease.Linear);

                Debug.Log("Wobble");
            }
        }
    }
}