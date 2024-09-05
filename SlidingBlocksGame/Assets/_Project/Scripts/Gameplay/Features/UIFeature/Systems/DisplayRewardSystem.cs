using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

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
            [Inc] public readonly EcsPool<OpenCloseTween> OpenCloseTween;

            [Inc] public readonly EcsPool<RewardWindowConnect> RewardWindowConnects;
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
                foreach (int reward in _world.Where(out RewardAspect aspect))
                {
                    ref RewardWindowConnect rewardWindowConnect = ref aspect.RewardWindowConnects.Get(reward);

                    rewardWindowConnect.Value.transform.localScale = Vector3.zero;
                    
                    rewardWindowConnect.Value.gameObject.SetActive(true);

                    aspect.OpenCloseTween.Get(reward).Value.Stop();

                    aspect.OpenCloseTween.Get(reward).Value = Tween.Scale(rewardWindowConnect.Value.transform,
                        Vector3.one, 0.3f, Ease.OutBack);
                }
            }

            foreach (int _ in _world.Where(out CloseRewardWindowButtonClickedAspect _))
            {
                foreach (int reward in _world.Where(out RewardAspect aspect))
                {
                    ref RewardWindowConnect rewardWindowConnect = ref aspect.RewardWindowConnects.Get(reward);

                    aspect.OpenCloseTween.Get(reward).Value.Stop();

                    aspect.OpenCloseTween.Get(reward).Value = Tween.Scale(rewardWindowConnect.Value.transform,
                            Vector3.zero, 0.5f, Ease.OutBack)
                        .OnComplete(rewardWindowConnect.Value, target => { target.gameObject.SetActive(false); });
                }
            }
        }
    }
}