using _Project.Scripts.Gameplay.Features.CollectFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using YG;

namespace _Project.Scripts.Gameplay.Features.CollectFeature
{
    public class RewardSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class RewardButtonClickedAspect : EcsAspectAuto
        {
            [Inc] private readonly EcsTagPool<RewardButtonTag> _rewardButtonTag;
            [Inc] private readonly EcsTagPool<ButtonClickedEvent> _buttonClickedEvent;
        }

        private class RewardAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(RewardTag))]
            [IncImplicit(typeof(CanRewardMarker))]
            [Inc] public readonly EcsPool<CoinsProgressionCurve> CoinsProgressionCurves;

            [Opt] public readonly EcsTagPool<RewardedEvent> Rewarded;
            [Opt] public readonly EcsTagPool<CoinsUpdatedEvent> CoinsUpdated;
            [Opt] public readonly EcsPool<TargetEntity> TargetEntity;
        }

        private class PlayerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayerTag))]
            [Inc] public readonly EcsPool<Coins> Coins;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out RewardButtonClickedAspect _))
            {
                foreach (int reward in _world.Where(out RewardAspect rewardAspect))
                {
                    foreach (int player in _world.Where(out PlayerAspect playerAspect))
                    {
                        ref Coins playerCoins = ref playerAspect.Coins.Get(player);

                        int rewardCoins = (int)rewardAspect.CoinsProgressionCurves.Read(reward).Value
                            .Evaluate(YandexGame.savesData.RewardCount);

                        playerCoins.Value += rewardCoins;

                        int @event = _world.NewEntity();
                        rewardAspect.Rewarded.Add(@event);
                        rewardAspect.CoinsUpdated.Add(@event);
                        rewardAspect.TargetEntity.Add(@event).Value = player.ToEntityLong(_world);
                    }
                }
            }
        }
    }
}