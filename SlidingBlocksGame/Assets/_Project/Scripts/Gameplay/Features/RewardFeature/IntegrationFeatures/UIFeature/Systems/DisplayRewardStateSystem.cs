using System;
using _Project.Scripts.Gameplay.Features.RewardFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;
using YG;

namespace _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.UIFeature.Systems
{
    public class DisplayRewardStateSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class RewardUnlockStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(RewardTag))]
            [IncImplicit(typeof(RewardEligibilityEvent))]
            [Inc] public readonly EcsPool<RewardWidget> RewardWidgets;
        }

        private class RewardLockStateAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(RewardTag))]
            [ExcImplicit(typeof(RewardEligibilityMarker))]
            [Inc] public readonly EcsPool<RewardWidget> RewardWidgets;

            [Inc] public readonly EcsPool<Reward> Rewards;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out RewardUnlockStateAspect aspect))
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
        }
    }
}