using _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.UIFeature.Systems;
using _Project.Scripts.Gameplay.Features.AudioBaseFeature.Components;
using _Project.Scripts.Gameplay.Features.AudioFeature;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.IntegrationFeatures.UIFeature.Systems;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.IntegrationFeatures.UIFeature.Systems;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Systems;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.IntegrationFeatures.UIFeature.Systems;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.Systems;
using _Project.Scripts.Gameplay.Features.RateUsFeature.Systems;
using _Project.Scripts.Gameplay.Features.RewardFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.UIFeature.Systems;
using _Project.Scripts.Gameplay.Features.RewardFeature.Systems;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.ButtonFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Systems;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature
{
    public class UIFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AddUnique(new DisplayTimerProgressSystem())
                //
                .AddUnique(new SettingsPopupSystem())
                .AutoDelTag<GameAudioUpdatedEvent>()
                .AddUnique(new UpdateGameAudioStatusSystem())
                .AddUnique(new DisplayAudioButtonsStatusSystem())
                .AutoDelEntityComponent<UpdateGameAudioRequest>()
                //
                .AddUnique(new CameraRenderSystem())
                .AddUnique(new DisplayPurchasePriceSystem())
                .AddUnique(new DisplayPurchaseStatusSystem())
                //
                .AddUnique(new DisplayAnimalPurchaseWindowSystem())
                .AutoDelTag<ClosedEvent>()
                .AddUnique(new CloseAnimalPurchaseWindowSystem())
                //
                .AutoDelTag<GameOverTimerClosedEvent>()
                .AddUnique(new CloseGameOverTimerSystem())
                .AutoDelTag<CloseGameOverTimerRequest>()
                //
                .AddUnique(new DisplayCoinsSystem())
                .AddUnique(new DisplayScoresSystem())
                //
                .AddUnique(new RateUsSystem())
                .AddUnique(new TutorialSystem())
                //
                .AutoDelTag<RewardCoinDisplayCompletedEvent>()
                .AutoDelEntityTag<RewardCoinCountDisplayedEvent>()
                .AutoDelTag<ConfettiExplodedEvent>()
                .AddUnique(new RewardCatcherSystem())
                .AddUnique(new DisplayRewardSystem())
                //
                .AddUnique(new PlayWidgetSystem())
                //
                .AddUnique(new WobbleSystem())
                .AutoDelTag<WobbleRequest>()
                //
                .AddUnique(new Render3DToUISystem())
                .AutoDel<Render3DToUIRequest>()
                //
                .AddModule(new ScrollSnapFeature.ScrollSnapFeature())
                .AutoDelEntityTag<ButtonClickedEvent>();
        }
    }
}