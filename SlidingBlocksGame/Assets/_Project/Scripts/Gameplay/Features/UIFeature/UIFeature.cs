using _Project.Scripts.Gameplay.Features.AudioBaseFeature.Components;
using _Project.Scripts.Gameplay.Features.AudioFeature;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Systems;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.Systems;
using _Project.Scripts.Gameplay.Features.RateUsFeature.Systems;
using _Project.Scripts.Gameplay.Features.RewardFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.Systems;
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Systems;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Systems;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature
{
    public class UIFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AddUnique(new DisplayProgressTimerSystem())
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
                .AutoDelTag<GameLossTimerClosedEvent>()
                .AddUnique(new CloseGameLossTimerSystem())
                .AutoDelTag<CloseGameLossTimerRequest>()
                //
                .AddUnique(new DisplayCoinsSystem())
                .AddUnique(new DisplayScoresSystem())
                //
                .AddUnique(new RateUsSystem())
                .AddUnique(new TutorialSystem())
                //
                .AutoDelTag<RewardCollectedEvent>()
                .AutoDelEntityTag<CoinAddedToTextEvent>()
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