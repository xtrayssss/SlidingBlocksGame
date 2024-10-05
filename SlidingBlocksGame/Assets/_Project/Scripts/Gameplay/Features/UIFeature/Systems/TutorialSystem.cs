using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class TutorialSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class OpenButtonClickedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<TutorialButtonTag> TutorialButtonTag;
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> Clicked;
        }

        private class CloseButtonClickedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CloseTutorialButtonTag> CloseTutorialButtonTag;
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> Clicked;
        }

        private class TutorialWindowAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(TutorialWindowTag))]
            [Inc] public readonly EcsPool<GameObjectConnect> GoConnects;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out OpenButtonClickedAspect _))
            {
                foreach (int window in _world.Where(out TutorialWindowAspect tutorialWindowAspect))
                {
                    ref GameObjectConnect goConnect = ref tutorialWindowAspect.GoConnects.Get(window);

                    goConnect.Connect.transform.localScale = Vector3.zero;

                    Tween.Scale(
                        target: goConnect.Connect.transform,
                        endValue: Vector3.one,
                        duration: 0.2f,
                        ease: Ease.OutBack);

                    goConnect.Connect.gameObject.SetActive(true);
                }
            }

            foreach (int _ in _world.Where(out CloseButtonClickedAspect _))
            {
                foreach (int window in _world.Where(out TutorialWindowAspect tutorialWindowAspect))
                {
                    ref GameObjectConnect goConnect = ref tutorialWindowAspect.GoConnects.Get(window);

                    Tween.Scale(
                            target: goConnect.Connect.transform,
                            endValue: Vector3.zero,
                            duration: 0.2f,
                            ease: Ease.InBack)
                        .OnComplete(
                            target: goConnect.Connect,
                            static connect => connect.gameObject.SetActive(false));
                }
            }
        }
    }
}