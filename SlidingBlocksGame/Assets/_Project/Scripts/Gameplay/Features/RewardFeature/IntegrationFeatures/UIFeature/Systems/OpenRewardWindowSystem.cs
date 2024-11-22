using _Project.Scripts.Gameplay.Features.CoinFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.UIFeature.Components;
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
    public class OpenRewardWindowSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private const float COIN_DELAY = 0.05f;

        private class OpenWindowRequestAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<RewardCollectedEvent> RewardCollectedEvent;
        }

        private class RewardAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(RewardTag))]
            [IncImplicit(typeof(RewardEligibilityMarker))]
            [Inc] public readonly EcsPool<RewardWidget> RewardWidgets;

            [Inc] public readonly EcsPool<RewardScalingCurve> RewardScalingCurve;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out OpenWindowRequestAspect _))
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

                    rewardWindow.OpenCloseTween = Sequence.Create()
                        .Chain(
                            Tween.Scale(
                                target: rewardWindowConnect.transform,
                                endValue: Vector3.one,
                                duration: 0.3f,
                                ease: Ease.OutBack))
                        .Chain(
                            AnimateRewardCoins(
                                    rewardWindowID,
                                    reward,
                                    out float animationDuration)
                                .InsertCallback(
                                    atTime: animationDuration * 0.8f,
                                    target: rewardWindow.RewardConfettiEffectConnect,
                                    static connect =>
                                    {
                                        connect.gameObject.SetActive(true);

                                        if (!connect.Entity.TryGetID(out int _))
                                            return;

                                        EcsWorld world = connect.Entity.World;

                                        int catcher = world.NewEntity();

                                        RewardCatcherAspect.ConfettiCatcher catcherAspect =
                                            world.GetAspect<RewardCatcherAspect.ConfettiCatcher>();
                                        catcherAspect.CommonCatcherAspect.TargetEntities.Add(catcher).Value =
                                            connect.Entity;
                                        catcherAspect.CatchConfettiExplodedRequest.Add(catcher);
                                    }))
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
                                rewardWindowID))
                        .ChainCallback(
                            rewardWindowConnect,
                            static connect =>
                            {
                                EcsWorld world = connect.World;

                                if (!connect.Entity.TryGetID(out int rewardWindowID))
                                    return;

                                RewardWindowAspect rewardWindowAspect = world.GetAspect<RewardWindowAspect>();

                                RewardWindow rewardWindow = rewardWindowAspect.RewardWindows.Get(rewardWindowID);

                                if (!rewardWindow.TapToExitWidgetConnect.Entity.TryGetID(out int tapToExitID))
                                    return;

                                TapToExitAspect tapToExitAspect = world.GetAspect<TapToExitAspect>();
                                ref TapToExitWidget widget = ref tapToExitAspect.TapToExitWidgets.Get(tapToExitID);
                                widget.ExitButton.interactable = true;
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
                            settings: UIUtils.WobbleLoopingSettings.settings);
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

                        widget.WobbleTween = Tween.UIAnchoredPosition(
                            target: uiElement.RectTransform,
                            endValue: uiElement.RectTransform.anchoredPosition + UIUtils.WOBBLE_OFFSET,
                            settings: UIUtils.WobbleLoopingSettings.settings);
                    });

            return sequence;
        }

        private Sequence AnimateRewardCoins(int rewardWindowID, int rewardID, out float animationDuration)
        {
            Sequence sequence = Sequence.Create();

            RewardWindowAspect rewardWindowAspect = _world.GetAspect<RewardWindowAspect>();

            ref readonly RewardWindow rewardWindow = ref rewardWindowAspect.RewardWindows.Read(rewardWindowID);

            rewardWindow.RewardCoinsWidgetConnect.gameObject.SetActive(true);

            RewardAspect rewardAspect = _world.GetAspect<RewardAspect>();

            int coins = (int)rewardAspect.RewardScalingCurve.Read(rewardID).Value
                .Evaluate(YandexGame.savesData.Savings.RewardCount);

            animationDuration = COIN_DELAY * coins;

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
                    duration: animationDuration,
                    frequency: 100));

            return sequence;
        }
    }
}