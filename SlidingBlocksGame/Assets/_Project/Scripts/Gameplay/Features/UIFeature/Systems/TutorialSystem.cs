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

        private class GameScreenAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameScreenTag))]
            [Inc] public readonly EcsPool<TutorialConnect> TutorialConnects;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out OpenButtonClickedAspect _))
            {
                foreach (int screen in _world.Where(out GameScreenAspect gameScreenAspect))
                {
                    ref TutorialConnect tutorialConnect = ref gameScreenAspect.TutorialConnects.Get(screen);

                    tutorialConnect.Value.transform.localScale = Vector3.zero;

                    Tween.Scale(
                        target: tutorialConnect.Value.transform,
                        endValue: Vector3.one,
                        duration: 0.2f,
                        ease: Ease.OutBack);

                    tutorialConnect.Value.gameObject.SetActive(true);
                }
            }

            foreach (int _ in _world.Where(out CloseButtonClickedAspect _))
            {
                foreach (int screen in _world.Where(out GameScreenAspect gameScreenAspect))
                {
                    ref TutorialConnect tutorialConnect = ref gameScreenAspect.TutorialConnects.Get(screen);

                    Tween.Scale(
                            target: gameScreenAspect.TutorialConnects.Get(screen).Value.transform,
                            endValue: Vector3.zero,
                            duration: 0.2f,
                            ease: Ease.InBack)
                        .OnComplete(
                            target: tutorialConnect.Value,
                            connect => connect.gameObject.SetActive(false));
                }
            }
        }
    }
}