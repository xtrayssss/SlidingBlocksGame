using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.RewardFeature.Utils
{
    public static class RewardUtils
    {
        private class Aspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<UpdateRewardRequest> UpdateReward;
            [Inc] public readonly EcsPool<TargetEntity> Rewardable;
        }

        public static void Update(int rewardable, UpdateRewardRequest data)
        {
            EcsDefaultWorld world = EcsDefaultWorldSingletonProvider.Instance.Get();

            Aspect aspect = world.GetAspect<Aspect>();

            int request = world.NewEntity();

            aspect.UpdateReward.Add(request) = data;
            aspect.Rewardable.Add(request).Value = rewardable.ToEntityLong(world);
        }
    }
}