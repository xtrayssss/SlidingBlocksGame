using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature
{
    public class UIFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AddUnique(new SettingsPopupSystem())
                //
                .AddUnique(new TutorialSystem())
                //
                .AddUnique(new PlayWidgetSystem())
                //
                .AddUnique(new Render3DToUISystem())
                .AutoDel<Render3DToUIRequest>()
                //
                .AutoDelTag<MetaGameUIHiddenEvent>()
                .AddUnique(new MetaGameUISystem())
                .AutoDelTag<ShowMetaGameUIRequest>()
                .AutoDelTag<HideMetaGameUIRequest>()
                //
                .AutoDelTag<GameScreenCreatedEvent>()
                .AddUnique(new CreateGameScreenSystem())
                .AutoDelTag<CreateGameScreenRequest>()
                //
                .AddUnique(new CalculateOriginalPositionSystem())
                .AutoDelTag<CalculateOriginalPositionRequest>()
                //
                .AddModule(new ScrollSnapFeature.ScrollSnapFeature())
                .AddModule(new ButtonFeature.ButtonFeature());
        }
    }
}