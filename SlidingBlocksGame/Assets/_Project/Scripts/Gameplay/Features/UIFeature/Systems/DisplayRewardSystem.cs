using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;
using YG;
using Sequence = PrimeTween.Sequence;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class DisplayRewardSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class RewardUnlockStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(RewardTag))]
            [IncImplicit(typeof(CanRewardMarker))]
            [Inc] public readonly EcsPool<RewardStatus> Status;
        }

        private class RewardLockStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(RewardTag))]
            [ExcImplicit(typeof(CanRewardMarker))]
            [Inc] public readonly EcsPool<RewardStatus> Status;
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

        private class RewardWindowAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<OpenCloseTween> OpenCloseTween;
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;
            [Inc] public readonly EcsPool<CoinsRewardConnect> CoinsRewardConnects;
            [Inc] public readonly EcsPool<RewardConfettiEffectConnect> RewardConfettiEffectConnect;
            [Inc] public EcsPool<CongratulationConnect> CongratulationConnects;
            [Inc] public EcsPool<SunshineConnect> SunshineConnects;
            [Inc] public EcsPool<TapToExitConnect> TapToExitConnects;
        }

        private class RewardCoinsAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<TextMeshProUGUIRef> TextMeshProUGUI;
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
            foreach (int entity in _world.Where(out RewardUnlockStateAspect aspect))
            {
                ref RewardStatus rewardStatus = ref aspect.Status.Get(entity);

                rewardStatus.Unlocked.gameObject.SetActive(true);
                rewardStatus.Locked.gameObject.SetActive(false);
            }

            foreach (int entity in _world.Where(out RewardLockStateAspect aspect))
            {
                ref RewardStatus rewardStatus = ref aspect.Status.Get(entity);

                rewardStatus.Locked.gameObject.SetActive(true);
                rewardStatus.Unlocked.gameObject.SetActive(false);
            }

            foreach (int _ in _world.Where(out RewardButtonClickedAspect _))
            {
                foreach (int reward in _world.Where(out RewardAspect rewardAspect))
                {
                    ref RewardWindowConnect rewardWindowConnect = ref rewardAspect.RewardWindowConnects.Get(reward);

                    rewardWindowConnect.Value.transform.localScale = Vector3.zero;

                    rewardWindowConnect.Value.gameObject.SetActive(true);

                    ref OpenCloseTween openCloseTween =
                        ref _world.GetPool<OpenCloseTween>().Get(rewardWindowConnect.Value.Entity.ID);

                    openCloseTween.Value.Stop();
                    Sequence sequence = Sequence.Create();

                    RewardCoinsAspect rewardCoinsAspect = _world.GetAspect<RewardCoinsAspect>();
                    RewardWindowAspect rewardWindowAspect = _world.GetAspect<RewardWindowAspect>();

                    openCloseTween.Value = sequence
                        .Chain(Tween.Scale(rewardWindowConnect.Value.transform,
                            Vector3.one, 0.3f, Ease.OutBack));

                    AnimateCoins(sequence, rewardWindowConnect.Value.Entity, reward, rewardAspect,
                            rewardCoinsAspect)
                        .ChainDelay(0.3f)
                        .Chain(Animate(rewardCoinsAspect, rewardAspect, reward, rewardWindowAspect));
                }
            }

            foreach (int _ in _world.Where(out CloseRewardWindowButtonClickedAspect _))
            {
                foreach (int window in _world.Where(out RewardWindowAspect aspect))
                {
                    ref GameObjectConnect gameObjectConnect = ref aspect.GameObjectConnects.Get(window);

                    ref OpenCloseTween openCloseTween = ref aspect.OpenCloseTween.Get(window);

                    openCloseTween.Value.Stop();

                    Sequence sequence = Sequence.Create();

                    RewardCoinsAspect rewardCoinsAspect = _world.GetAspect<RewardCoinsAspect>();
                    RewardWindowAspect rewardWindowAspect = _world.GetAspect<RewardWindowAspect>();

                    openCloseTween.Value =
                        sequence
                            .Group(Tween.Scale(gameObjectConnect.Connect.transform, Vector3.zero, 0.2f, Ease.InBack))
                            .Group(AnimateRollback(rewardCoinsAspect, rewardWindowAspect, window))
                            .ChainCallback(
                                // confetti
                                target: rewardWindowAspect.RewardConfettiEffectConnect.Get(window).Value,
                                connect => connect.gameObject.SetActive(false))
                            .ChainCallback(
                                target: gameObjectConnect.Connect,
                                connect => connect.gameObject.SetActive(false));
                }
            }
        }

        private Sequence AnimateRollback(RewardCoinsAspect rewardCoinsAspect, RewardWindowAspect rewardWindowAspect,
            int window)
        {
            Sequence sequence = Sequence.Create();

            ref GameObjectConnect rewardWindowConnect = ref rewardWindowAspect.GameObjectConnects.Get(window);

            if (!rewardWindowConnect.Connect.Entity.TryGetID(out int rewardWindowID))
                return sequence;
            
            // coins reward
            ref readonly CoinsRewardConnect coinsRewardConnect =
                ref rewardWindowAspect.CoinsRewardConnects.Read(rewardWindowID);

            sequence.ChainCallback(coinsRewardConnect.Value, connect =>
            {
                if (!connect.Entity.TryGetID(out int coinsRewardID))
                    return;

                rewardCoinsAspect.WobbleTween.Get(coinsRewardID).Value.Stop();
            });

            // congratulation
            ref CongratulationConnect congratulationConnect = ref rewardWindowAspect.CongratulationConnects
                .Get(rewardWindowID);

            sequence
                .Group(Tween.Scale(congratulationConnect.Value.transform, Vector3.zero, 0.15f, Ease.InBack))
                .ChainCallback(
                    target: congratulationConnect.Value,
                    connect => connect.gameObject.SetActive(false));

            // sunshine 
            ref SunshineConnect sunshineConnect =
                ref rewardWindowAspect.SunshineConnects.Get(rewardWindowID);

            sequence
                .Group(Tween.Scale(sunshineConnect.Value.transform, Vector3.zero, 0.15f, Ease.InBack))
                .ChainCallback(
                    target: sunshineConnect.Value,
                    connect => connect.gameObject.SetActive(false));

            // tap to exit
            ref TapToExitConnect tapToExitConnect = ref rewardWindowAspect.TapToExitConnects.Get(rewardWindowID);

            sequence
                .ChainCallback(
                    target: tapToExitConnect.Value,
                    connect =>
                    {
                        if (!connect.Entity.TryGetID(out int tapToExitID))
                            return;

                        _world.GetPool<WobbleTween>().Get(tapToExitID).Value.Stop();
                        _world.GetPool<ButtonRef>().Get(tapToExitID).Value.interactable = false;
                    })
                .Group(Tween.Scale(tapToExitConnect.Value.transform, Vector3.zero, 0.15f, Ease.InBack))
                .ChainCallback(
                    target: tapToExitConnect.Value,
                    connect => connect.gameObject.SetActive(false));

            return sequence;
        }

        private Sequence Animate(RewardCoinsAspect rewardCoinsAspect,
            RewardAspect rewardAspect, int reward,
            RewardWindowAspect rewardWindowAspect)
        {
            Sequence sequence = Sequence.Create();

            ref RewardWindowConnect rewardWindowConnect = ref rewardAspect.RewardWindowConnects.Get(reward);

            if (!rewardWindowConnect.Value.Entity.TryGetID(out int rewardWindowID))
                return sequence;

            // confetti
            sequence.ChainCallback(
                rewardWindowAspect.RewardConfettiEffectConnect.Get(rewardWindowID).Value,
                connect => connect.gameObject.SetActive(true));

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
                .Group(Tween.Scale(sunshineConnect.Value.transform, new Vector3(2, 2, 2), 0.5f, Ease.OutBack));

            sequence.ChainCallback(
                sunshineConnect.Value,
                connect => Tween.LocalRotationAtSpeed(connect.transform,
                    Quaternion.Euler(new Vector3(0, 0, 360)), 70f, Ease.Linear, cycles: -1,
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

        private Sequence AnimateCoins(Sequence sequence, entlong rewardWindow, int reward, RewardAspect rewardAspect,
            RewardCoinsAspect rewardCoinsAspect)
        {
            Sequence animationSequence = Sequence.Create();

            RewardWindowAspect rewardWindowAspect = _world.GetAspect<RewardWindowAspect>();

            ref CoinsRewardConnect coinsRewardConnect = ref rewardWindowAspect.CoinsRewardConnects.Get(rewardWindow.ID);
            coinsRewardConnect.Value.gameObject.SetActive(true);

            int coins = (int)rewardAspect.CoinsProgressionCurves.Read(reward).Value
                .Evaluate(YandexGame.savesData.RewardCount);

            for (int i = 0; i < coins; i++)
            {
                int i1 = i;

                animationSequence.Chain(
                    Tween.Delay(0.05f)
                        .OnComplete(rewardCoinsAspect.TextMeshProUGUI.Get(coinsRewardConnect.Value.Entity.ID).Value,
                            text => { text.text = (i1 + 1).ToString(); }));
            }

            sequence.Chain(animationSequence);

            sequence.Group(
                Tween.ShakeLocalPosition(rewardCoinsAspect.TextMeshProUGUI
                        .Get(coinsRewardConnect.Value.Entity.ID).Value.gameObject.transform, strength: Vector3.one * 20,
                    coins * 0.05f, frequency: 100));

            return sequence;
        }
    }
}