using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    public class WobbleSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;
        
        private static readonly Vector2 WOBBLE_OFFSET = new Vector2(0, 45);

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

                ref WobbleTween wobbleTween = ref aspect.WobbleTween.TryAddOrGet(entity);
                
                wobbleTween.Value = Tween.UIAnchoredPosition(
                    target: rect.Value,
                    endValue: rect.Value.anchoredPosition + WOBBLE_OFFSET,
                    duration: 1.3f,
                    cycles: -1,
                    cycleMode: CycleMode.Yoyo,
                    ease: Ease.Linear);

                Debug.Log("Wobble");
            }
        }
    }
}