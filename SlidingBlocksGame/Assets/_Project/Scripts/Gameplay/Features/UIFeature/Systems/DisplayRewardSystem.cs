using System;
using _Project.Scripts.Gameplay.Features.CollectFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;
using YG;
using IEcsRun = DCFApixels.DragonECS.IEcsRun;
using Sequence = PrimeTween.Sequence;
using Tween = PrimeTween.Tween;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class DisplayRewardSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class RewardUnlockStateAspect
        {
            public class OnEnter : EcsAspectAuto
            {
                [IncImplicit(typeof(RewardTag))]
                [IncImplicit(typeof(CanRewardEvent))]
                [Inc] public readonly EcsPool<RewardStatus> Status;

                [Inc] public readonly EcsPool<GrabRewardText> GrabRewardText;
                [Inc] public readonly EcsPool<RewardTimeText> RewardTimeText;
                [Inc] public readonly EcsPool<GrabRewardTextTween> GrabRewardTextTween;
            }
        }

        private class RewardLockStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(RewardTag))]
            [ExcImplicit(typeof(CanRewardMarker))]
            [Inc] public readonly EcsPool<RewardStatus> Status;

            [Inc] public readonly EcsPool<RewardTimeText> RewardTimeText;
            [Inc] public readonly EcsPool<RewardCollectedAt> RewardCollectedAt;
            [Inc] public readonly EcsPool<GrabRewardText> GrabRewardText;
            [Inc] public readonly EcsPool<RewardInterval> RewardInterval;
        }

        private class RewardButtonClickedAspect : EcsAspectAuto
        {
            [Inc] private readonly EcsTagPool<RewardButtonTag> _rewardButtonTag;
            [Inc] private readonly EcsTagPool<ButtonClickedEvent> _buttonClickedEvent;
        }

        private class RewardAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(RewardTag))]
            [IncImplicit(typeof(CanRewardMarker))]
            [Inc] public readonly EcsPool<RewardWindowConnect> RewardWindowConnects;

            [Inc] public readonly EcsPool<CoinsProgressionCurve> CoinsProgressionCurves;
        }

        public class RewardWindowAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<OpenCloseSequence> OpenCloseTween;
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;
            [Inc] public readonly EcsPool<CoinsRewardConnect> CoinsRewardConnects;
            [Inc] public readonly EcsPool<RewardConfettiEffectConnect> RewardConfettiEffectConnect;
            [Inc] public readonly EcsPool<CongratulationConnect> CongratulationConnects;
            [Inc] public readonly EcsPool<SunshineConnect> SunshineConnects;
            [Inc] public readonly EcsPool<TapToExitConnect> TapToExitConnects;
        }

        private class RewardCoinsAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<TextMeshProUGUIRef> TextMeshProUGUI;
            [Inc] public readonly EcsPool<Coins> CoinsDisplay;
            [Opt] public readonly EcsTagPool<WobbleRequest> Wobble;
            [Opt] public readonly EcsPool<WobbleTween> WobbleTween;
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
                ref RewardStatus rewardStatus = ref aspect.Status.Get(entity);

                rewardStatus.Unlocked.gameObject.SetActive(true);
                rewardStatus.Locked.gameObject.SetActive(false);

                ref GrabRewardText grabRewardText = ref aspect.GrabRewardText.Get(entity);
                grabRewardText.Value.gameObject.SetActive(true);
                aspect.RewardTimeText.Get(entity).Value.gameObject.SetActive(false);

                ref GrabRewardTextTween tween = ref aspect.GrabRewardTextTween.Get(entity);

                Debug.Log("RewardUnlockStateAspect.OnEnter");

                tween.Value.Stop();

                grabRewardText.Value.transform.localScale = Vector3.one;

                tween.Value = Tween.Scale(
                    target: grabRewardText.Value.transform,
                    endValue: new Vector3(1.15f, 1.15f, 1),
                    duration: 0.5f,
                    ease: Ease.InOutSine,
                    cycles: -1,
                    cycleMode: CycleMode.Yoyo);
            }

            foreach (int entity in _world.Where(out RewardLockStateAspect aspect))
            {
                ref RewardStatus rewardStatus = ref aspect.Status.Get(entity);

                rewardStatus.Locked.gameObject.SetActive(true);
                rewardStatus.Unlocked.gameObject.SetActive(false);

                long difference = YandexGame.ServerTime() - aspect.RewardCollectedAt.Read(entity).Value;

                difference = Math.Max(0, difference);

                string time = TimeSpan
                    .FromMilliseconds(aspect.RewardInterval.Get(entity).Value - difference)
                    .ToString(@"hh\:mm\:ss");

                aspect.RewardTimeText.Get(entity).Value.text = "REWARD IN: " + time;

                aspect.GrabRewardText.Get(entity).Value.gameObject.SetActive(false);
                aspect.RewardTimeText.Get(entity).Value.gameObject.SetActive(true);
            }


            foreach (int _ in _world.Where(out RewardButtonClickedAspect _))
            {
                foreach (int reward in _world.Where(out RewardAspect rewardAspect))
                {
                    ref RewardWindowConnect rewardWindowConnect = ref rewardAspect.RewardWindowConnects.Get(reward);

                    rewardWindowConnect.Value.transform.localScale = Vector3.zero;

                    rewardWindowConnect.Value.gameObject.SetActive(true);

                    ref OpenCloseSequence openCloseSequence =
                        ref _world.GetPool<OpenCloseSequence>().Get(rewardWindowConnect.Value.Entity.ID);

                    openCloseSequence.Value.Stop();

                    Sequence sequence = Sequence.Create();

                    RewardCoinsAspect rewardCoinsAspect = _world.GetAspect<RewardCoinsAspect>();
                    RewardWindowAspect rewardWindowAspect = _world.GetAspect<RewardWindowAspect>();

                    openCloseSequence.Value = sequence
                        .Chain(
                            Tween.Scale(
                                target: rewardWindowConnect.Value.transform,
                                endValue: Vector3.one,
                                duration: 0.3f,
                                ease: Ease.OutBack))
                        .Chain(
                            AnimateCoins(
                                rewardWindowConnect.Value.Entity,
                                reward,
                                rewardAspect,
                                rewardCoinsAspect,
                                rewardWindowConnect.Value.Entity.ID))
                        .ChainCallback(
                            target: rewardWindowConnect.Value,
                            static connect =>
                            {
                                if (!connect.Entity.TryGetID(out int _))
                                    return;

                                EcsWorld world = connect.Entity.World;

                                int catcher = world.NewEntity();

                                RewardCatcherAspect.RewardCollectedCatcher catcherAspect =
                                    world.GetAspect<RewardCatcherAspect.RewardCollectedCatcher>();

                                catcherAspect.CommonCatcherAspect.TargetEntities.Add(catcher).Value = connect.Entity;
                                catcherAspect.CatchRewardCollectedRequest.Add(catcher);
                            })
                        .ChainDelay(0.3f)
                        .Chain(
                            Animate(
                                rewardCoinsAspect,
                                rewardAspect,
                                reward,
                                rewardWindowAspect));
                }
            }

            foreach (int _ in _world.Where(out CloseRewardWindowButtonClickedAspect _))
            {
                foreach (int window in _world.Where(out RewardWindowAspect aspect))
                {
                    ref GameObjectConnect gameObjectConnect = ref aspect.GameObjectConnects.Get(window);

                    ref OpenCloseSequence openCloseSequence = ref aspect.OpenCloseTween.Get(window);

                    openCloseSequence.Value.Stop();

                    Sequence sequence = Sequence.Create();

                    RewardWindowAspect rewardWindowAspect = _world.GetAspect<RewardWindowAspect>();

                    openCloseSequence.Value =
                        sequence
                            .Group(
                                Tween.Scale(
                                    target: gameObjectConnect.Connect.transform,
                                    endValue: Vector3.zero,
                                    duration: 0.2f,
                                    ease: Ease.InBack))
                            .Group(
                                AnimateRollback(
                                    rewardWindowAspect,
                                    window))
                            // close confetti
                            .ChainCallback(
                                target: rewardWindowAspect.RewardConfettiEffectConnect.Get(window).Value,
                                connect => connect.gameObject.SetActive(false))
                            // cleanup reward coins
                            .ChainCallback(
                                target: gameObjectConnect.Connect,
                                static connect =>
                                {
                                    if (!connect.Entity.TryGetID(out int rewardWindowID))
                                        return;

                                    EcsWorld world = connect.Entity.World;

                                    RewardWindowAspect rewardWindowAspect = world.GetAspect<RewardWindowAspect>();

                                    ref readonly CoinsRewardConnect coinsRewardConnect =
                                        ref rewardWindowAspect.CoinsRewardConnects.Read(rewardWindowID);

                                    if (!coinsRewardConnect.Value.Entity.TryGetID(out int coinsRewardID))
                                        return;

                                    RewardCoinsAspect rewardCoinsAspect = world.GetAspect<RewardCoinsAspect>();

                                    rewardCoinsAspect.TextMeshProUGUI.Get(coinsRewardID).Value.text = "0";
                                })
                            // close window
                            .ChainCallback(
                                target: gameObjectConnect.Connect,
                                static connect => connect.gameObject.SetActive(false));

                    Debug.Log(openCloseSequence.Value.durationTotal);
                }
            }
        }

        private Sequence Animate(RewardCoinsAspect rewardCoinsAspect,
            RewardAspect rewardAspect, int reward,
            RewardWindowAspect rewardWindowAspect)
        {
            Sequence sequence = Sequence.Create();

            ref RewardWindowConnect rewardWindowConnect = ref rewardAspect.RewardWindowConnects.Get(reward);

            if (!rewardWindowConnect.Value.Entity.TryGetID(out int rewardWindowID))
                return sequence;

            // coins reward
            ref readonly CoinsRewardConnect coinsRewardConnect =
                ref rewardWindowAspect.CoinsRewardConnects.Read(rewardWindowID);

            sequence.ChainCallback(coinsRewardConnect.Value, connect =>
            {
                if (!connect.Entity.TryGetID(out int coinsRewardID))
                    return;

                rewardCoinsAspect.Wobble.Add(coinsRewardID);
            });

            // congratulation
            ref CongratulationConnect congratulationConnect = ref rewardWindowAspect.CongratulationConnects
                .Get(rewardWindowID);

            congratulationConnect.Value.transform.localScale = Vector3.zero;

            sequence.Group(Tween.Scale(congratulationConnect.Value.transform, Vector3.one, 0.5f, Ease.OutBack));

            congratulationConnect.Value.gameObject.SetActive(true);

            // sunshine 
            ref SunshineConnect sunshineConnect =
                ref rewardWindowAspect.SunshineConnects.Get(rewardWindowID);

            sunshineConnect.Value.gameObject.SetActive(true);

            sunshineConnect.Value.transform.localScale = Vector3.zero;

            sequence
                .Group(Tween.Scale(
                    target: sunshineConnect.Value.transform,
                    endValue: Vector3.one,
                    duration: 0.5f,
                    ease: Ease.OutBack));

            sequence.ChainCallback(
                target: sunshineConnect.Value,
                connect => Tween.LocalEulerAngles(
                    target: connect.transform,
                    startValue: connect.transform.localRotation.eulerAngles,
                    endValue: new Vector3(0, 0, 360),
                    duration: 3f,
                    ease: Ease.Linear,
                    cycles: -1,
                    cycleMode: CycleMode.Incremental));

            // tap to exit
            ref TapToExitConnect tapToExitConnect = ref rewardWindowAspect.TapToExitConnects.Get(rewardWindowID);

            tapToExitConnect.Value.gameObject.SetActive(true);

            tapToExitConnect.Value.transform.localScale = Vector3.zero;

            sequence
                .Group(Tween.Scale(tapToExitConnect.Value.transform, Vector3.one, 0.5f, Ease.OutBack))
                .ChainCallback(
                    target: tapToExitConnect.Value,
                    connect =>
                    {
                        if (!connect.Entity.TryGetID(out int tapToExitID))
                            return;

                        _world.GetPool<WobbleRequest>().Add(tapToExitID);
                        _world.GetPool<ButtonRef>().Get(tapToExitID).Value.interactable = true;
                    });

            return sequence;
        }

        private Sequence AnimateRollback(RewardWindowAspect rewardWindowAspect, int window)
        {
            Sequence sequence = Sequence.Create();

            ref GameObjectConnect rewardWindowConnect = ref rewardWindowAspect.GameObjectConnects.Get(window);

            if (!rewardWindowConnect.Connect.Entity.TryGetID(out int rewardWindowID))
                return sequence;

            // coins reward
            ref readonly CoinsRewardConnect coinsRewardConnect =
                ref rewardWindowAspect.CoinsRewardConnects.Read(rewardWindowID);

            sequence
                .ChainCallback(
                    target: coinsRewardConnect.Value,
                    static connect =>
                    {
                        if (!connect.Entity.TryGetID(out int coinsRewardID))
                            return;

                        EcsWorld world = connect.Entity.World;

                        RewardCoinsAspect rewardCoinsAspect = world.GetAspect<RewardCoinsAspect>();

                        rewardCoinsAspect.WobbleTween.Get(coinsRewardID).Value.Stop();
                    });

            // congratulation
            ref CongratulationConnect congratulationConnect = ref rewardWindowAspect.CongratulationConnects
                .Get(rewardWindowID);

            sequence
                .Group(
                    Tween.Scale(
                        target: congratulationConnect.Value.transform,
                        endValue: Vector3.zero,
                        duration: 0.15f,
                        ease: Ease.InBack))
                .ChainCallback(
                    target: congratulationConnect.Value,
                    static connect => connect.gameObject.SetActive(false));

            // sunshine 
            ref SunshineConnect sunshineConnect =
                ref rewardWindowAspect.SunshineConnects.Get(rewardWindowID);

            sequence
                .Group(
                    Tween.Scale(
                        target: sunshineConnect.Value.transform,
                        endValue: Vector3.zero,
                        duration: 0.15f,
                        ease: Ease.InBack))
                .ChainCallback(
                    target: sunshineConnect.Value,
                    static connect => connect.gameObject.SetActive(false));

            // tap to exit
            ref TapToExitConnect tapToExitConnect = ref rewardWindowAspect.TapToExitConnects.Get(rewardWindowID);

            sequence
                .ChainCallback(
                    target: tapToExitConnect.Value,
                    static connect =>
                    {
                        if (!connect.Entity.TryGetID(out int tapToExitID))
                            return;

                        EcsWorld world = connect.Entity.World;

                        world.GetPool<WobbleTween>().Get(tapToExitID).Value.Stop();
                        world.GetPool<ButtonRef>().Get(tapToExitID).Value.interactable = false;
                    })
                .Group(
                    Tween.Scale(
                        target: tapToExitConnect.Value.transform,
                        Vector3.zero,
                        duration: 0.15f,
                        Ease.InBack))
                .ChainCallback(
                    target: tapToExitConnect.Value,
                    static connect => connect.gameObject.SetActive(false));

            return sequence;
        }

        private Sequence AnimateCoins(entlong window, int reward, RewardAspect rewardAspect,
            RewardCoinsAspect rewardCoinsAspect, int rewardWindowID)
        {
            Sequence sequence = Sequence.Create();

            RewardWindowAspect rewardWindowAspect = _world.GetAspect<RewardWindowAspect>();

            ref CoinsRewardConnect coinsRewardConnect = ref rewardWindowAspect.CoinsRewardConnects.Get(window.ID);
            coinsRewardConnect.Value.gameObject.SetActive(true);

            int coins = (int)rewardAspect.CoinsProgressionCurves.Read(reward).Value
                .Evaluate(YandexGame.savesData.RewardCount);

            const float coinDelay = 0.05f;

            float totalCoinDelay = coinDelay * coins;

            if (coinsRewardConnect.Value.Entity.TryGetID(out int coinsRewardID))
                rewardCoinsAspect.CoinsDisplay.Get(coinsRewardID).Value = 0;

            for (int i = 0; i < coins; i++)
            {
                sequence
                    .Chain(
                        Tween.Delay(coinDelay))
                    .ChainCallback(
                        target: coinsRewardConnect.Value,
                        static connect =>
                        {
                            if (!connect.Entity.TryGetID(out int id))
                                return;

                            EcsWorld world = connect.Entity.World;

                            RewardCoinsAspect rewardCoinsAspect = world.GetAspect<RewardCoinsAspect>();

                            rewardCoinsAspect.TextMeshProUGUI.Get(id).Value.text =
                                (++world.GetPool<Coins>().Get(id).Value).ToString();

                            int catcher = world.NewEntity();

                            RewardCatcherAspect.CoinAddedToTextCatcher catcherAspect =
                                world.GetAspect<RewardCatcherAspect.CoinAddedToTextCatcher>();

                            catcherAspect.CommonCatcherAspect.TargetEntities.Add(catcher).Value = connect.Entity;
                            catcherAspect.CatchCoinAddedToTextRequest.Add(catcher);
                        });
            }

            sequence.Group(
                Tween.ShakeLocalPosition(
                    target: rewardCoinsAspect.TextMeshProUGUI.Get(coinsRewardConnect.Value.Entity.ID).Value.transform,
                    strength: Vector3.one * 20,
                    duration: totalCoinDelay,
                    frequency: 100));

            float percent = totalCoinDelay * 0.8f;

            sequence.InsertCallback(
                atTime: percent,
                target: rewardWindowAspect.RewardConfettiEffectConnect.Get(rewardWindowID).Value,
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