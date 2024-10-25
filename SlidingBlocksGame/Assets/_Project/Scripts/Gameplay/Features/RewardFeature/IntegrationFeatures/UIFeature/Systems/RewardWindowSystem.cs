using System;
using _Project.Scripts.Gameplay.Features.CoinFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.ButtonFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Utils;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;
using YG;
using IEcsRun = DCFApixels.DragonECS.IEcsRun;
using Sequence = PrimeTween.Sequence;
using Tween = PrimeTween.Tween;

namespace _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.UIFeature.Systems
{
    public class RewardWindowSystem : IEcsRun
    {
        private const float COIN_DELAY = 0.05f;
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class RewardUnlockStateAspect
        {
            public class OnEnter : EcsAspectAuto
            {
                [IncImplicit(typeof(RewardTag))]
                [IncImplicit(typeof(RewardEligibilityEvent))]
                [Inc] public readonly EcsPool<RewardWidget> RewardWidgets;
            }
        }

        private class RewardLockStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(RewardTag))]
            [ExcImplicit(typeof(RewardEligibilityMarker))]
            [Inc] public readonly EcsPool<RewardWidget> RewardWidgets;

            [Inc] public readonly EcsPool<Reward> Rewards;
        }

        private class OpenRewardButtonClickedAspect : EcsAspectAuto
        {
            [Inc] private readonly EcsTagPool<RewardButtonTag> _rewardButtonTag;
            [Inc] private readonly EcsTagPool<ButtonClickedEvent> _buttonClickedEvent;
        }

        private class RewardAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(RewardTag))]
            [IncImplicit(typeof(RewardEligibilityMarker))]
            [Inc] public readonly EcsPool<RewardWidget> RewardWidgets;

            [Inc] public readonly EcsPool<RewardScalingCurve> RewardScalingCurve;
        }

        private class RewardWindowAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameObjectConnect> GoConnects;
            [Inc] public readonly EcsPool<RewardWindow> RewardWindows;
        }

        private class RewardCoinsAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Coins> CoinsDisplay;
            [Inc] public readonly EcsPool<RewardCoinsWidget> RewardCoinsWidgets;
            [Inc] public readonly EcsPool<UIElement> UIElements;
            [Inc] public readonly EcsPool<OriginalAnchoredPosition> OriginalAnchoredPositions;
        }

        private class TapToExitAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<TapToExitWidget> TapToExitWidgets;
            [Inc] public readonly EcsPool<OriginalAnchoredPosition> OriginalAnchoredPositions;
            [Inc] public readonly EcsPool<UIElement> UIElements;
        }
        
        private class SunshineAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Sunshine> Sunshine;
        }

        private class CloseRewardWindowButtonClickedAspect : EcsAspectAuto
        {
            [Inc] private readonly EcsTagPool<CloseRewardWindowButtonTag> _closeRewardWindowButton;
            [Inc] private readonly EcsTagPool<ButtonClickedEvent> _buttonClicked;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out RewardUnlockStateAspect.OnEnter aspect))
            {
                ref RewardWidget rewardWidget = ref aspect.RewardWidgets.Get(entity);

                rewardWidget.Unlocked.gameObject.SetActive(true);
                rewardWidget.Locked.gameObject.SetActive(false);

                rewardWidget.ClaimRewardWidget.ClaimRewardText.gameObject.SetActive(true);
                rewardWidget.ClaimRewardWidget.RewardTimeText.gameObject.SetActive(false);

                ref Tween tween = ref rewardWidget.ClaimRewardWidget.Tween;

                tween.Stop();

                rewardWidget.ClaimRewardWidget.ClaimRewardText.transform.localScale = Vector3.one;

                tween = Tween.Scale(
                    target: rewardWidget.ClaimRewardWidget.ClaimRewardText.transform,
                    endValue: new Vector3(1.15f, 1.15f, 1),
                    duration: 0.5f,
                    ease: Ease.InOutSine,
                    cycles: -1,
                    cycleMode: CycleMode.Yoyo);
            }

            foreach (int entity in _world.Where(out RewardLockStateAspect aspect))
            {
                ref RewardWidget rewardWidget = ref aspect.RewardWidgets.Get(entity);

                rewardWidget.Unlocked.gameObject.SetActive(false);
                rewardWidget.Locked.gameObject.SetActive(true);

                ref readonly Reward reward = ref aspect.Rewards.Read(entity);

                long diff = YandexGame.ServerTime() - reward.CollectionTime;

                diff = Math.Max(0, diff);

                string time = TimeSpan
                    .FromMilliseconds(reward.Interval - diff)
                    .ToString(@"hh\:mm\:ss");

                rewardWidget.ClaimRewardWidget.RewardTimeText.text = "REWARD IN: " + time;

                rewardWidget.ClaimRewardWidget.ClaimRewardText.gameObject.SetActive(false);
                rewardWidget.ClaimRewardWidget.RewardTimeText.gameObject.SetActive(true);
            }

            foreach (int _ in _world.Where(out OpenRewardButtonClickedAspect _))
            {
                foreach (int reward in _world.Where(out RewardAspect rewardAspect))
                {
                    RewardWindowAspect rewardWindowAspect = _world.GetAspect<RewardWindowAspect>();

                    ref RewardWidget rewardWidget = ref rewardAspect.RewardWidgets.Get(reward);

                    EcsEntityConnect rewardWindowConnect = rewardWidget.RewardWindowConnect;

                    rewardWindowConnect.transform.localScale = Vector3.zero;

                    rewardWindowConnect.gameObject.SetActive(true);

                    if (!rewardWindowConnect.Entity.TryGetID(out int rewardWindowID))
                        continue;

                    RewardWindow rewardWindow = rewardWindowAspect.RewardWindows.Get(rewardWindowID);

                    rewardWindow.OpenCloseTween.Stop();

                    Sequence sequence = Sequence.Create();

                    rewardWindow.OpenCloseTween = sequence
                        .Chain(
                            Tween.Scale(
                                target: rewardWindowConnect.transform,
                                endValue: Vector3.one,
                                duration: 0.3f,
                                ease: Ease.OutBack))
                        .Chain(
                            AnimateCoins(
                                    rewardWindowID,
                                    reward)
                                .ChainCallback(
                                    target: rewardWindowConnect,
                                    static connect =>
                                    {
                                        if (!connect.Entity.TryGetID(out int _))
                                            return;

                                        EcsWorld world = connect.Entity.World;

                                        int catcher = world.NewEntity();

                                        RewardCatcherAspect.CoinDisplayCompletedCatcher catcherAspect =
                                            world.GetAspect<RewardCatcherAspect.CoinDisplayCompletedCatcher>();

                                        catcherAspect.CommonCatcherAspect.TargetEntities.Add(catcher).Value =
                                            connect.Entity;
                                        catcherAspect.CatchRewardCoinDisplayCompletedRequest.Add(catcher);
                                    })
                                .ChainDelay(0.3f)
                                .Chain(
                                    Animate(
                                        rewardWindowID)));
                }
            }

            foreach (int _ in _world.Where(out CloseRewardWindowButtonClickedAspect _))
            {
                foreach (int window in _world.Where(out RewardWindowAspect rewardWindowAspect))
                {
                    ref RewardWindow rewardWindow = ref rewardWindowAspect.RewardWindows.Get(window);

                    ref GameObjectConnect goConnect = ref rewardWindowAspect.GoConnects.Get(window);

                    rewardWindow.OpenCloseTween.Stop();

                    Sequence sequence = Sequence.Create();
                    
                    if (rewardWindow.TapToExitWidgetConnect.Entity.TryGetID(out int tapToExitID))
                    {
                        TapToExitAspect tapToExitAspect = _world.GetAspect<TapToExitAspect>();
                        ref TapToExitWidget tapToExitWidget = ref tapToExitAspect.TapToExitWidgets.Get(tapToExitID);
                        tapToExitWidget.ExitButton.interactable = false;
                    }

                    rewardWindow.OpenCloseTween =
                        sequence
                            .Group(
                                Tween.Scale(
                                    target: goConnect.Connect.transform,
                                    endValue: Vector3.zero,
                                    duration: 0.2f,
                                    ease: Ease.InBack))
                            .Group(
                                AnimateRollback(
                                    window))
                            // close confetti
                            .ChainCallback(
                                target: rewardWindow.RewardConfettiEffectConnect,
                                connect => connect.gameObject.SetActive(false))
                            // cleanup reward coins
                            .ChainCallback(
                                target: goConnect.Connect,
                                static connect =>
                                {
                                    if (!connect.Entity.TryGetID(out int rewardWindowID))
                                        return;

                                    EcsWorld world = connect.Entity.World;

                                    RewardWindowAspect rewardWindowAspect = world.GetAspect<RewardWindowAspect>();

                                    ref RewardWindow rewardWindow =
                                        ref rewardWindowAspect.RewardWindows.Get(rewardWindowID);

                                    if (!rewardWindow.RewardCoinsWidgetConnect.Entity.TryGetID(out int coinsRewardID))
                                        return;

                                    RewardCoinsAspect rewardCoinsAspect = world.GetAspect<RewardCoinsAspect>();

                                    RewardCoinsWidget rewardCoinsWidget =
                                        rewardCoinsAspect.RewardCoinsWidgets.Get(coinsRewardID);

                                    rewardCoinsWidget.AmountText.text = "0";
                                })
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
                                    }

                                    if (rewardWindow.SunshineConnect.Entity.TryGetID(out int sunshineID))
                                    {
                                        SunshineAspect rewardCoinsAspect = world.GetAspect<SunshineAspect>();

                                        ref Sunshine sunshine = ref rewardCoinsAspect.Sunshine.Get(sunshineID);
                                        sunshine.RotationTween.Stop();
                                    }
                                });
                }
            }
        }

        private Sequence Animate(int rewardWindowID)
        {
            Sequence sequence = Sequence.Create();

            RewardWindowAspect rewardWindowAspect = _world.GetAspect<RewardWindowAspect>();

            ref readonly RewardWindow rewardWindow = ref rewardWindowAspect.RewardWindows.Read(rewardWindowID);

            // coins reward
            sequence
                .ChainCallback(
                    rewardWindow.RewardCoinsWidgetConnect,
                    static connect =>
                    {
                        if (!connect.Entity.TryGetID(out int id))
                            return;

                        EcsWorld world = connect.Entity.World;

                        RewardCoinsAspect rewardCoinsAspect = world.GetAspect<RewardCoinsAspect>();

                        ref UIElement uiElement = ref rewardCoinsAspect.UIElements.Get(id);
                        ref RewardCoinsWidget widget = ref rewardCoinsAspect.RewardCoinsWidgets.Get(id);

                        widget.WobbleTween = Tween.UIAnchoredPosition(
                            target: uiElement.RectTransform,
                            endValue: uiElement.RectTransform.anchoredPosition + UIUtils.WOBBLE_OFFSET,
                            settings: UIUtils.WobbleSettings.settings);
                    });

            // congratulation
            rewardWindow.CongratulationWidgetConnect.transform.localScale = Vector3.zero;

            sequence
                .Group(
                    Tween.Scale(
                        target: rewardWindow.CongratulationWidgetConnect.transform,
                        endValue: Vector3.one,
                        duration: 0.5f,
                        ease: Ease.OutBack));

            rewardWindow.CongratulationWidgetConnect.gameObject.SetActive(true);

            // sunshine 
            rewardWindow.SunshineConnect.gameObject.SetActive(true);

            rewardWindow.SunshineConnect.transform.localScale = Vector3.zero;

            sequence
                .Group(Tween.Scale(
                    target: rewardWindow.SunshineConnect.transform,
                    endValue: Vector3.one,
                    duration: 0.5f,
                    ease: Ease.OutBack));

            sequence.ChainCallback(
                target: rewardWindow.SunshineConnect,
                static connect =>
                {
                    if (!connect.Entity.TryGetID(out int id))
                        return;

                    EcsWorld world = connect.World;

                    SunshineAspect rewardCoinsAspect = world.GetAspect<SunshineAspect>();
                    ref Sunshine sunshine = ref rewardCoinsAspect.Sunshine.Get(id);

                    sunshine.RotationTween = Tween.LocalEulerAngles(
                        target: connect.transform,
                        startValue: connect.transform.localRotation.eulerAngles,
                        endValue: connect.transform.localRotation.eulerAngles + new Vector3(0, 0, 360),
                        duration: 3f,
                        ease: Ease.Linear,
                        cycles: -1,
                        cycleMode: CycleMode.Incremental);
                });

            // tap to exit
            rewardWindow.TapToExitWidgetConnect.gameObject.SetActive(true);

            rewardWindow.TapToExitWidgetConnect.transform.localScale = Vector3.zero;

            sequence
                .Group(
                    Tween.Scale(
                        target: rewardWindow.TapToExitWidgetConnect.transform,
                        endValue: Vector3.one,
                        duration: 0.5f,
                        ease: Ease.OutBack))
                .ChainCallback(
                    target: rewardWindow.TapToExitWidgetConnect,
                    static connect =>
                    {
                        if (!connect.Entity.TryGetID(out int id))
                            return;

                        EcsWorld world = connect.Entity.World;

                        TapToExitAspect tapToExitAspect = world.GetAspect<TapToExitAspect>();
                        ref UIElement uiElement = ref tapToExitAspect.UIElements.Get(id);

                        ref TapToExitWidget widget = ref tapToExitAspect.TapToExitWidgets.Get(id);

                        widget.ExitButton.interactable = true;

                        widget.WobbleTween = Tween.UIAnchoredPosition(
                            target: uiElement.RectTransform,
                            endValue: uiElement.RectTransform.anchoredPosition + UIUtils.WOBBLE_OFFSET,
                            settings: UIUtils.WobbleSettings.settings);
                    });

            return sequence;
        }

        private Sequence AnimateRollback(int rewardWindowID)
        {
            Sequence sequence = Sequence.Create();

            RewardWindowAspect rewardWindowAspect = _world.GetAspect<RewardWindowAspect>();

            ref readonly RewardWindow rewardWindow = ref rewardWindowAspect.RewardWindows.Read(rewardWindowID);

            // coins reward
            sequence
                .ChainCallback(
                    target: rewardWindow.RewardCoinsWidgetConnect,
                    static connect =>
                    {
                        if (!connect.Entity.TryGetID(out int coinsRewardID))
                            return;

                        EcsWorld world = connect.Entity.World;

                        RewardCoinsAspect rewardCoinsAspect = world.GetAspect<RewardCoinsAspect>();

                        ref RewardCoinsWidget widget = ref rewardCoinsAspect.RewardCoinsWidgets.Get(coinsRewardID);

                        widget.WobbleTween.isPaused = true;
                    });

            // congratulation
            sequence
                .Group(
                    Tween.Scale(
                        target: rewardWindow.CongratulationWidgetConnect.transform,
                        endValue: Vector3.zero,
                        duration: 0.15f,
                        ease: Ease.InBack))
                .ChainCallback(
                    target: rewardWindow.CongratulationWidgetConnect,
                    static connect => connect.gameObject.SetActive(false));

            // sunshine 
            sequence
                .Group(
                    Tween.Scale(
                        target: rewardWindow.SunshineConnect.transform,
                        endValue: Vector3.zero,
                        duration: 0.15f,
                        ease: Ease.InBack))
                .ChainCallback(
                    target: rewardWindow.SunshineConnect,
                    static connect => connect.gameObject.SetActive(false));

            // tap to exit
            sequence
                .ChainCallback(
                    target: rewardWindow.TapToExitWidgetConnect,
                    static connect =>
                    {
                        if (!connect.Entity.TryGetID(out int tapToExitID))
                            return;

                        EcsWorld world = connect.Entity.World;

                        TapToExitAspect tapToExitAspect = world.GetAspect<TapToExitAspect>();

                        ref TapToExitWidget widget = ref tapToExitAspect.TapToExitWidgets.Get(tapToExitID);

                        widget.WobbleTween.isPaused = true;

                        widget.ExitButton.interactable = false;
                    })
                .Group(
                    Tween.Scale(
                        target: rewardWindow.TapToExitWidgetConnect.gameObject.transform,
                        Vector3.zero,
                        duration: 0.15f,
                        Ease.InBack))
                .ChainCallback(
                    target: rewardWindow.TapToExitWidgetConnect,
                    static connect => connect.gameObject.SetActive(false));

            return sequence;
        }

        private Sequence AnimateCoins(int rewardWindowID, int rewardID)
        {
            Sequence sequence = Sequence.Create();

            RewardWindowAspect rewardWindowAspect = _world.GetAspect<RewardWindowAspect>();

            ref readonly RewardWindow rewardWindow = ref rewardWindowAspect.RewardWindows.Read(rewardWindowID);

            rewardWindow.RewardCoinsWidgetConnect.gameObject.SetActive(true);

            RewardAspect rewardAspect = _world.GetAspect<RewardAspect>();

            int coins = (int)rewardAspect.RewardScalingCurve.Read(rewardID).Value
                .Evaluate(YandexGame.savesData.RewardCount);

            float totalCoinDelay = COIN_DELAY * coins;

            // reward coins
            RewardCoinsAspect rewardCoinsAspect = _world.GetAspect<RewardCoinsAspect>();

            if (rewardWindow.RewardCoinsWidgetConnect.Entity.TryGetID(out int coinsRewardID))
                rewardCoinsAspect.CoinsDisplay.Get(coinsRewardID).Value = 0;

            for (int i = 0; i < coins; i++)
            {
                sequence
                    .Chain(
                        Tween.Delay(COIN_DELAY))
                    .ChainCallback(
                        target: rewardWindow.RewardCoinsWidgetConnect,
                        static connect =>
                        {
                            if (!connect.Entity.TryGetID(out int id))
                                return;

                            EcsWorld world = connect.Entity.World;

                            RewardCoinsAspect rewardCoinsAspect = world.GetAspect<RewardCoinsAspect>();

                            ref RewardCoinsWidget widget = ref rewardCoinsAspect.RewardCoinsWidgets.Get(id);
                            widget.AmountText.text = (++world.GetPool<Coins>().Get(id).Value).ToString();

                            int catcher = world.NewEntity();

                            RewardCatcherAspect.CoinCountDisplayedCatcher catcherAspect =
                                world.GetAspect<RewardCatcherAspect.CoinCountDisplayedCatcher>();

                            catcherAspect.CommonCatcherAspect.TargetEntities.Add(catcher).Value = connect.Entity;
                            catcherAspect.CatchRewardCoinCountDisplayedRequest.Add(catcher);
                        });
            }

            sequence.Group(
                Tween.ShakeLocalPosition(
                    target: rewardWindow.RewardCoinsWidgetConnect.transform,
                    strength: Vector3.one * 20,
                    duration: totalCoinDelay,
                    frequency: 100));

            float percent = totalCoinDelay * 0.8f;

            // confetti
            sequence.InsertCallback(
                atTime: percent,
                target: rewardWindow.RewardConfettiEffectConnect,
                static connect =>
                {
                    connect.gameObject.SetActive(true);

                    if (!connect.Entity.TryGetID(out _))
                        return;

                    EcsWorld world = connect.Entity.World;

                    int catcher = world.NewEntity();

                    RewardCatcherAspect.ConfettiCatcher catcherAspect =
                        world.GetAspect<RewardCatcherAspect.ConfettiCatcher>();
                    catcherAspect.CommonCatcherAspect.TargetEntities.Add(catcher).Value = connect.Entity;
                    catcherAspect.CatchConfettiExplodedRequest.Add(catcher);
                });

            return sequence;
        }
    }
}