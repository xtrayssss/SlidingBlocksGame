using _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.ButtonFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.UIFeature.Systems
{
    public class CloseRewardWindowSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class CloseWindowClickedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CloseRewardWindowButtonTag> CloseRewardWindowButton;
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> ButtonClicked;
        }
        
        public void Run()
        {
            foreach (int _ in _world.Where(out CloseWindowClickedAspect _))
            {
                foreach (int window in _world.Where(out RewardWindowAspect rewardWindowAspect))
                {
                    ref RewardWindow rewardWindow = ref rewardWindowAspect.RewardWindows.Get(window);

                    ref GameObjectConnect goConnect = ref rewardWindowAspect.GoConnects.Get(window);

                    if (rewardWindow.TapToExitWidgetConnect.Entity.TryGetID(out int tapToExitID))
                    {
                        TapToExitAspect tapToExitAspect = _world.GetAspect<TapToExitAspect>();
                        ref TapToExitWidget tapToExitWidget = ref tapToExitAspect.TapToExitWidgets.Get(tapToExitID);
                        tapToExitWidget.ExitButton.interactable = false;
                    }

                    rewardWindow.OpenCloseTween.Stop();

                    Sequence sequence = Sequence.Create();

                    rewardWindow.OpenCloseTween =
                        sequence
                            .Group(
                                Tween.Scale(
                                    target: goConnect.Connect.transform,
                                    endValue: Vector3.zero,
                                    duration: 0.2f,
                                    ease: Ease.InBack))
                            // close window
                            .ChainCallback(
                                target: goConnect.Connect,
                                static connect =>
                                {
                                    connect.gameObject.SetActive(false);

                                    if (!connect.Entity.TryGetID(out int id))
                                        return;

                                    EcsWorld world = connect.World;

                                    RewardWindowAspect windowAspect = world.GetAspect<RewardWindowAspect>();
                                    ref RewardWindow rewardWindow = ref windowAspect.RewardWindows.Get(id);

                                    if (rewardWindow.TapToExitWidgetConnect.Entity.TryGetID(out int tapToExitID))
                                    {
                                        TapToExitAspect tapToExitAspect = world.GetAspect<TapToExitAspect>();

                                        ref TapToExitWidget widget =
                                            ref tapToExitAspect.TapToExitWidgets.Get(tapToExitID);
                                        widget.WobbleTween.Stop();

                                        ref UIElement uiElement = ref tapToExitAspect.UIElements.Get(tapToExitID);
                                        ref readonly OriginalAnchoredPosition originalAnchoredPosition =
                                            ref tapToExitAspect.OriginalAnchoredPositions.Read(tapToExitID);
                                        uiElement.RectTransform.anchoredPosition = originalAnchoredPosition.Value;
                                    }

                                    if (rewardWindow.RewardCoinsWidgetConnect.Entity.TryGetID(out int rewardCoinsID))
                                    {
                                        RewardCoinsAspect rewardCoinsAspect = world.GetAspect<RewardCoinsAspect>();

                                        ref RewardCoinsWidget widget =
                                            ref rewardCoinsAspect.RewardCoinsWidgets.Get(rewardCoinsID);
                                        widget.WobbleTween.Stop();

                                        ref UIElement uiElement = ref rewardCoinsAspect.UIElements.Get(rewardCoinsID);
                                        ref readonly OriginalAnchoredPosition originalAnchoredPosition =
                                            ref rewardCoinsAspect.OriginalAnchoredPositions.Read(rewardCoinsID);

                                        uiElement.RectTransform.anchoredPosition = originalAnchoredPosition.Value;

                                        widget.AmountText.text = "0";
                                    }

                                    if (rewardWindow.SunshineConnect.Entity.TryGetID(out int sunshineID))
                                    {
                                        SunshineAspect rewardCoinsAspect = world.GetAspect<SunshineAspect>();

                                        ref Sunshine sunshine = ref rewardCoinsAspect.Sunshine.Get(sunshineID);
                                        sunshine.RotationTween.Stop();
                                        rewardWindow.SunshineConnect.gameObject.SetActive(false);
                                    }

                                    if (rewardWindow.RewardConfettiEffectConnect.Entity.TryGetID(out var _))
                                        rewardWindow.RewardConfettiEffectConnect.gameObject.SetActive(false);
                                });
                }
            }
        }
    }
}