using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CooldownFeature
{
    public class CooldownFeature<TMask> : EcsModule<TMask> where TMask : EcsAspect, new()
    {
        protected override void Import(Builder builder)
        {
            builder
                .AddSystem(new RefreshCooldownSystem())
                .AddSystem(new DeleteEntityOnExpiredSystem())
                .AutoDelTag<CooldownExpiredEvent>()
                .AddSystem(new CountdownSystem())
                .AddSystem(new CooldownSystem())
                .AutoDelTag<CooldownTickEvent>()
                .AddSystem(new CooldownIntervalSystem())
                .AutoDelTag<RefreshCooldownRequest>();
        }
    }
}