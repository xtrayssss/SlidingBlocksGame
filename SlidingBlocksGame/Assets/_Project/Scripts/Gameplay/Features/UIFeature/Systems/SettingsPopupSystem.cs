using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;
using YG;

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

        private class CloseButtonClickedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> _;
            [Inc] public readonly EcsTagPool<CloseSettingsButtonTag> _1;
        }

        private class GameScreenAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameScreenTag))]
            [Inc] public readonly EcsPool<SettingsPopupConnect> SettingsMenuViews;
        }

        private class SettingsPopupAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(SettingsPopupTag))]
            [Inc] public readonly EcsPool<OpenCloseTween> OpenCloseTween;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out OpenPopupButtonClickedAspect _))
            {
                foreach (int gameScreen in _world.Where(out GameScreenAspect gameScreenAspect))
                {
                    ref readonly SettingsPopupConnect
                        connect = ref gameScreenAspect.SettingsMenuViews.Read(gameScreen);

                    if (!connect.Value.Entity.TryGetID(out int popupID))
                        return;

                    SettingsPopupAspect settingsPopupAspect = _world.GetAspect<SettingsPopupAspect>();

                    ref OpenCloseTween openCloseTween = ref settingsPopupAspect.OpenCloseTween.Get(popupID);

                    connect.Value.transform.localScale = Vector3.zero;

                    connect.Value.gameObject.SetActive(true);

                    openCloseTween.Value.Stop();

                    openCloseTween.Value =
                        Tween.Scale(connect.Value.transform, Vector3.one, 0.2f, Ease.OutBack);
                }
            }

            foreach (int _ in _world.Where(out CloseButtonClickedAspect _))
            {
                foreach (int gameScreen in _world.Where(out GameScreenAspect gameScreenAspect))
                {
                    ref readonly SettingsPopupConnect
                        connect = ref gameScreenAspect.SettingsMenuViews.Read(gameScreen);

                    if (!connect.Value.Entity.TryGetID(out int popupID))
                        return;

                    SettingsPopupAspect settingsPopupAspect = _world.GetAspect<SettingsPopupAspect>();

                    ref OpenCloseTween openCloseTween = ref settingsPopupAspect.OpenCloseTween.Get(popupID);

                    openCloseTween.Value.Stop();

                    openCloseTween.Value =
                        Tween
                            .Scale(connect.Value.transform, Vector3.zero, 0.2f, Ease.InBack)
                            .OnComplete(
                                target: connect.Value,
                                onComplete: entityConnect => entityConnect.gameObject.SetActive(false));
                }
            }
        }
    }
}