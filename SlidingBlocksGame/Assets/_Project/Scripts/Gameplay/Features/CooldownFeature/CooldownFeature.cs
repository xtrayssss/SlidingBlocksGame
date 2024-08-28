using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Systems;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CooldownFeature
{
    public class CooldownFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AddUnique(new RefreshCooldownSystem())
                .AddUnique(new DeleteEntityCommandOnExpiredSystem())
                .AutoDelTag<CooldownExpiredEvent>()
                .AddUnique(new CountdownSystem())
                .AddUnique(new CooldownSystem())
                .AddUnique(new CooldownIntervalSystem())
                .AutoDelTag<RefreshCooldownRequest>();
        }
    }
}