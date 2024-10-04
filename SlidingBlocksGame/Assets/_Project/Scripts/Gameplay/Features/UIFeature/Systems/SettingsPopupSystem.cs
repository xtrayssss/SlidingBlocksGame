using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class SettingsPopupSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class OpenPopupButtonClickedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> ButtonClicked;
            [Inc] public readonly EcsTagPool<SettingsButtonTag> SettingsButtonTag;
        }

        private class ClosePopupButtonClickedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> _;
            [Inc] public readonly EcsTagPool<CloseSettingsButtonTag> _1;
        }

        private class SettingsPopupAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(SettingsPopupTag))]
            [Inc] public readonly EcsPool<SettingsPopup> SettingsPopups;
            [Inc] public readonly EcsPool<GameObjectConnect> GoConnects;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out OpenPopupButtonClickedAspect _))
            {
                foreach (int popup in _world.Where(out SettingsPopupAspect settingsPopupAspect))
                {
                    ref SettingsPopup settingsPopup = ref settingsPopupAspect.SettingsPopups.Get(popup);
                    ref GameObjectConnect goConnect = ref settingsPopupAspect.GoConnects.Get(popup);

                    goConnect.Connect.transform.localScale = Vector3.zero;

                    goConnect.Connect.gameObject.SetActive(true);

                    settingsPopup.OpenCloseTween.Stop();

                    settingsPopup.OpenCloseTween =
                        Tween.Scale(
                            target: goConnect.Connect.transform,
                            endValue: Vector3.one,
                            duration: 0.2f,
                            ease: Ease.OutBack);
                }
            }

            foreach (int _ in _world.Where(out ClosePopupButtonClickedAspect _))
            {
                foreach (int popup in _world.Where(out SettingsPopupAspect settingsPopupAspect))
                {
                    ref SettingsPopup settingsPopup = ref settingsPopupAspect.SettingsPopups.Get(popup);
                    ref GameObjectConnect goConnect = ref settingsPopupAspect.GoConnects.Get(popup);

                    settingsPopup.OpenCloseTween.Stop();

                    settingsPopup.OpenCloseTween =
                        Tween
                            .Scale(
                                target: goConnect.Connect.transform,
                                endValue: Vector3.zero,
                                duration: 0.2f,
                                ease: Ease.InBack)
                            .OnComplete(
                                target: goConnect.Connect,
                                onComplete: static connect => connect.gameObject.SetActive(false));
                }
            }
        }
    }
}