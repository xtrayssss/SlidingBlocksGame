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
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref RectTransformRef rect = ref aspect.RectTransforms.Get(entity);
                
                Tween.UIAnchoredPosition(rect.Value,
                    rect.Value.anchoredPosition + new Vector2(0, 60), 1.3f, cycles: -1, cycleMode: CycleMode.Yoyo, ease: Ease.Linear);

                Debug.Log("Wobble");
            }
        }
    }
}