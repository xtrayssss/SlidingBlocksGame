using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Systems;
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Systems;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Systems;
using _Project.Scripts.Gameplay.Features.VisualFeature.VFXFeature;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.VisualFeature
{
    public class VisualFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AddModule(new VfxFeature())
                .AddModule(new UIFeature());
        }

        private class UIFeature : IEcsModule
        {
            public void Import(EcsPipeline.Builder builder)
            {
                builder
                    .AddUnique(new DisplayProgressTimerSystem())
                    //
                    .AddUnique(new SettingsPopupSystem())
                    .AutoDelTag<GameAudioUpdatedEvent>()
                    .AddUnique(new UpdateGameAudioSystem())
                    .AddUnique(new DisplayAudioButtonsStatusSystem())
                    .AutoDelEntityComponent<UpdateGameAudioRequest>()
                    //
                    .AddUnique(new InAppPopupSystem())
                    //
                    .AddUnique(new ScrollSnapSystem())
                    .AutoDel<ScrollSetupRequest>()
                    .AutoDelTag<ApplyEffectRequest>()
                    //
                    .AddUnique(new RotatePurchaseSystem())
                    .AddUnique(new CameraRenderSystem())
                    .AddUnique(new DisplayPriceAnimalSystem())
                    .AddUnique(new DisplayPurchaseStatusSystem())
                    //
                    .AutoDelTag<OpenedEvent>()
                    .AddUnique(new DisplayAnimalPurchaseWindowSystem())
                    .AutoDelTag<ClosedStartEvent>()
                    .AutoDelTag<ClosedEvent>()
                    .AddUnique(new CloseAnimalPurchaseWindowSystem())
                    //
                    .AutoDelTag<GameLossTimerClosedEvent>()
                    .AddUnique(new CloseGameLossTimerSystem())
                    .AutoDelTag<CloseGameLossTimerRequest>()
                    //
                    .AddUnique(new DisplayProgressSystem())
                    .AddUnique(new RateUsSystem())
                    .AddUnique(new TutorialSystem())
                    //
                    .AutoDelTag<RewardCollectedEvent>()
                    .AutoDelEntityTag<CoinAddedToTextEvent>()
                    .AutoDelTag<ConfettiExplodedEvent>()
                    .AddUnique(new RewardCatcherSystem())
                    .AddUnique(new DisplayRewardSystem())
                    //
                    .AutoDelTag<CanRewardEvent>()
                    .AddUnique(new CanRewardSystem())
                    .AddUnique(new WobbleSystem())
                    .AutoDelTag<WobbleRequest>()
                    //
                    .AutoDelEntityTag<ButtonClickedEvent>();
            }
        }
    }
}