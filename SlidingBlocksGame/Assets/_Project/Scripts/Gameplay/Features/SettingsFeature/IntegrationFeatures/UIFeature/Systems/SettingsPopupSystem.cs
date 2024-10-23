using _Project.Scripts.Gameplay.Features.GameScreenFeature.Components;
using _Project.Scripts.Gameplay.Features.SettingsFeature.Components;
using _Project.Scripts.Gameplay.Features.SettingsFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.ButtonFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.SettingsFeature.IntegrationFeatures.UIFeature.Systems
{
    public class SettingsPopupSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class SettingsCreatedEventAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<SettingsCreatedEvent> SettingsCreatedEvent;
        }

        private class GameScreenAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameScreen> GameScreens;
        }

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
            foreach (int settings in _world.Where(out SettingsCreatedEventAspect _))
            {
                foreach (int screen in _world.Where(out GameScreenAspect gameScreenAspect))
                {
                    ref GameScreen gameScreen = ref gameScreenAspect.GameScreens.Get(screen);
                    
                    gameScreen.SettingsPopupConnect.Connect(settings.ToEntityLong(_world), applyTemplates: true);
                }
            }

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