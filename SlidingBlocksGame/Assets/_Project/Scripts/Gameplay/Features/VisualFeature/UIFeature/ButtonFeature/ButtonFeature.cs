using _Project.Scripts.Gameplay.Features.AudioFeature.Extensions;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.ButtonFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.ButtonFeature.IntegrationFeatures.AudioFeature.
    Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.ButtonFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.ButtonFeature
{
    public class ButtonFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AutoDelEntityTag<ButtonClickedEvent>()
                .AddUnique(new CatchButtonEventsSystem())

                // audio feature
                .AddAudioSystem<ButtonClickedEvent, ButtonClickedAudioConfig>();
        }
    }
}