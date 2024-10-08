using _Project.Scripts.Gameplay.Features.GameProgressFeature.Systems;
using _Project.Scripts.Gameplay.Features.RewardFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.Systems;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.RewardFeature
{
    public class RewardFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AutoDelTag<RewardEligibilityEvent>()
                .AddUnique(new RewardEligibilitySystem())
                .AddUnique(new RewardClaimSystem());
        }
    }
}