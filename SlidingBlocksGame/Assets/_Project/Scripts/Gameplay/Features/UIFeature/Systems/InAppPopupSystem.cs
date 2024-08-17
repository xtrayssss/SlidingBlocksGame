using System;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    [Serializable]
    public class InAppPopupSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class OpenButtonAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> _;
            [Inc] public readonly EcsTagPool<InAppPopupButtonTag> _1;
        }

        private class CloseButtonAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> _;
            [Inc] public readonly EcsTagPool<CloseInAppPopupButtonTag> _1;
        }

        private class GameScreenAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameScreenTag))]
            [Inc] public readonly EcsPool<InAppPopupView> Popups;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out OpenButtonAspect aspect))
            {
                foreach (int gameScreen in _world.Where(out GameScreenAspect gameScreenAspect))
                {
                    ref readonly InAppPopupView popupView = ref gameScreenAspect.Popups.Read(gameScreen);

                    popupView.Value.transform.localScale = Vector3.zero;

                    popupView.Value.gameObject.SetActive(true);

                    Sequence.Create()
                        .Chain(Tween.Scale(popupView.Value.transform, Vector3.one * 1.2f, 0.1f, Ease.OutQuad)
                            .Chain(Tween.Scale(popupView.Value.transform, Vector3.one * 1f, 0.05f,
                                Ease.InQuad)));
                }
            }

            foreach (int entity in _world.Where(out CloseButtonAspect aspect))
            {
                foreach (int gameScreen in _world.Where(out GameScreenAspect gameScreenAspect))
                {
                    ref readonly InAppPopupView popupView = ref gameScreenAspect.Popups.Read(gameScreen);

                    InAppPopupView view = popupView;

                    Sequence.Create()
                        .Chain(Tween.Scale(popupView.Value.transform, Vector3.one * 1.2f, 0.1f, Ease.OutQuad)
                            .Chain(Tween.Scale(popupView.Value.transform, Vector3.zero, 0.05f,
                                Ease.InQuad))).ChainCallback(() => view.Value.gameObject.SetActive(false));
                }
            }
        }
    }
}