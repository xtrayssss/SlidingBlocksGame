using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Systems;
using _Project.Scripts.Gameplay.Features.VisualFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.Systems;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.VisualFeature
{
    public class VisualFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AddModule(new FxFeature())
                .AddModule(new UIFeature());
        }

        private class UIFeature : IEcsModule
        {
            public void Import(EcsPipeline.Builder builder)
            {
                builder
                    .AddUnique(new DisplayProgressTimerSystem())
                    .AddUnique(new SettingsPopupSystem())
                    .AddUnique(new AudioButtonsSystem())
                    .AddUnique(new InAppPopupSystem())
                    //
                    .AddUnique(new ScrollSystem())
                    .AutoDelTag<ScrollSetupRequest>()
                    .AutoDelTag<ApplyEffectRequest>()
                    //
                    .AddUnique(new RotationSystem())
                    .AddUnique(new CameraRenderSystem())
                    .AutoDelTag<ViewUpdatedEvent>()
                    .AddUnique(new DisplayPriceAnimalSystem())
                    .AddUnique(new DisplayPurchaseStatusSystem())
                    .AddUnique(new DisplayAnimalPurchaseWindowSystem())
                    .AddUnique(new CloseAnimalPurchaseWindowSystem())
                    .AddUnique(new DisplayProgressSystem())
                    .AddUnique(new RateUsSystem())
                    .AddUnique(new TutorialSystem())
                    //
                    .AutoDelTag<ConfettiExplodedEvent>()
                    .AutoDelTag<RewardCollectedEvent>()
                    .AutoDelEntityTag<CoinAddedToTextEvent>()
                    .AddUnique(new DisplayRewardSystem())
                    //
                    .AddUnique(new CanRewardSystem())
                    .AddUnique(new WobbleSystem())
                    .AutoDelTag<WobbleRequest>()
                    .AutoDelTag<ScrollStartedEvent>()
                    .AutoDelTag<ScrollSnappedEvent>();
            }
        }

        private class FxFeature : IEcsModule
        {
            public void Import(EcsPipeline.Builder builder)
            {
                builder
                    .AddUnique(new PlayFxSystem())
                    .AutoDelTag<PlayFxRequest>()
                    .AddUnique(new DestroyFxRequestSystem())
                    .AddUnique(new DestructionFxSystem());
            }
        }
    }
}