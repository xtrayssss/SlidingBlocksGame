using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    public class WobbleSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;
        
        private static readonly Vector2 WOBBLE_OFFSET = new Vector2(0, 45);

        private class WobblableAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(WobbleRequest))]
            [Inc] public readonly EcsPool<GameObjectConnect> GoConnects;

            [Opt] public readonly EcsPool<WobbleTween> WobbleTween;
        }

        public void Run()
        {
            foreach (int wobblable in _world.Where(out WobblableAspect wobblableAspect))
            {
                ref GameObjectConnect goConnect = ref wobblableAspect.GoConnects.Get(wobblable);
                RectTransform rectTransform = (RectTransform)goConnect.Connect.transform;

                ref WobbleTween wobbleTween = ref wobblableAspect.WobbleTween.TryAddOrGet(wobblable);
                
                wobbleTween.Value = Tween.UIAnchoredPosition(
                    target: rectTransform,
                    endValue: rectTransform.anchoredPosition + WOBBLE_OFFSET,
                    duration: 1.3f,
                    cycles: -1,
                    cycleMode: CycleMode.Yoyo,
                    ease: Ease.Linear);

                Debug.Log("Wobble");
            }
        }
    }
}