using _Project.Scripts.Gameplay.Features.SettingsFeature.Components;
using _Project.Scripts.Gameplay.Features.SettingsFeature.IntegrationFeatures.UIFeature.Systems;
using _Project.Scripts.Gameplay.Features.SettingsFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.SettingsFeature
{
    public class SettingsFeature<TMask> : EcsModule<TMask> where TMask : EcsAspect, new()
    {
        protected override void Import(Builder builder)
        {
            builder
                .AutoDelTag<SettingsCreatedEvent>()
                .AddUnique(new CreateSettingsSystem())
                .AutoDelTag<CreateSettingsRequest>()
                .AddUnique(new SettingsPopupSystem());
        }
    }
}