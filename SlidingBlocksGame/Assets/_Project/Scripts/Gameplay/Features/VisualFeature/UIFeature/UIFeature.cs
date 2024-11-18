using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature
{
    public class UIFeature<TMask> : EcsModule<TMask> where TMask : EcsAspect, new()
    {
        protected override void Import(Builder builder)
        {
            builder
                .AutoDelTag<MetaGameUIHiddenEvent>()
                .AddSystem(new MetaGameUISystem())
                .AutoDelTag<ShowMetaGameUIRequest>()
                .AutoDelTag<HideMetaGameUIRequest>()
                .AddSystem(new CalculateOriginalPositionSystem())
                .AutoDelTag<CalculateOriginalPositionRequest>()
                //
                .AddSubmodule<ScrollSnapFeature.ScrollSnapFeature<TMask>>()
                .AddSubmodule<ButtonFeature.ButtonFeature<TMask>>();
        }
    }
}