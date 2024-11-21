using _Project.Scripts.Gameplay.Features.TutorialFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.ButtonFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.TutorialFeature.IntegrationFeatures.UIFeature.Systems
{
    public class TutorialSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class OpenRequestAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<TutorialWindowTag> TutorialWindowTag;
            [Inc] public readonly EcsTagPool<OpenTutorialRequest> OpenTutorialRequest;
        }

        private class OpenButtonClickedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<TutorialButtonTag> TutorialButtonTag;
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> ButtonClickedEvent;
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

            [Opt] public readonly EcsTagPool<OpenTutorialRequest> OpenTutorialRequest;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out OpenButtonClickedAspect _))
            {
                foreach (int tutorial in _world.Where(out TutorialWindowAspect tutorialAspect))
                    tutorialAspect.OpenTutorialRequest.Add(tutorial);
            }

            foreach (int _ in _world.Where(out OpenRequestAspect _))
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