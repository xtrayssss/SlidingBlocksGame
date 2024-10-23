using _Project.Scripts.Gameplay.Features.GameAudioFeature.Components;
using _Project.Scripts.Gameplay.Features.GameAudioFeature.IntegrationFeatures.UIFeature.Systems;
using _Project.Scripts.Gameplay.Features.GameAudioFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameAudioFeature
{
    public class GameAudioFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AddUnique(new SwitchAudioSystem())
                //
                .AutoDelTag<AudioSettingsUpdatedEvent>()
                .AddUnique(new UpdateAudioSettingsSystem())

                // ui feature
                .AddUnique(new DisplayAudioButtonsStatusSystem());
        }
    }
}