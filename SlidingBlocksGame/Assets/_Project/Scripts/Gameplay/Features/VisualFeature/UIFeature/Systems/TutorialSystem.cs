using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.ButtonFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Systems
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

                    Tween.Scale(tutorialConnect.Value.transform, Vector3.one, 0.2f, Ease.OutBack);

                    tutorialConnect.Value.gameObject.SetActive(true);
                }
            }

            foreach (int _ in _world.Where(out CloseButtonClickedAspect _))
            {
                foreach (int screen in _world.Where(out GameScreenAspect gameScreenAspect))
                {
                    ref TutorialConnect tutorialConnect = ref gameScreenAspect.TutorialConnects.Get(screen);

                    Tween.Scale(gameScreenAspect.TutorialConnects.Get(screen).Value.transform, Vector3.zero, 0.2f,
                            Ease.InBack)
                        .OnComplete(tutorialConnect.Value, target => { target.gameObject.SetActive(false); });
                }
            }
        }
    }
}