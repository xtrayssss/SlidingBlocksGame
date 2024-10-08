using _Project.Scripts.Gameplay.Features.AnimalFeature.Components;
using _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.Systems;
using _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;
using Object = UnityEngine.Object;
using ScrollSnap = _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components.ScrollSnap;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.Systems
{
    public class GameScreenSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class GameAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CreateGameScreenRequest))]
            [Inc] public readonly EcsPool<GameScreenPrefab> GameScreenPrefabs;
        }

        private class PlayerAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<AnimalPrefabs> AnimalPrefabs;
        }

        private class GameScreenAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameScreen> GameScreens;
            [Opt] public readonly EcsTagPool<GameScreenCreatedEvent> GameScreenCreatedEvent;
        }

        private class AnimalsShopWindowAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<AnimalsShopWindow> AnimalsShopWindows;
            [Inc] public readonly EcsPool<Purchases> Purchases;
            [Inc] public readonly EcsPool<ScrollSnap> ScrollSnaps;
            [Opt] public readonly EcsPool<ScrollSetupRequest> ScrollSetupRequest;
        }

        private class RewardWidgetAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<RewardWidget> RewardWidgets;
        }

        private class RewardWindowAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<RewardWindow> RewardWindows;
        }

        private class GameTitleWidgetAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsTagPool<WobbleRequest> Wobble;
        }

        private class PurchaseAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsPool<PurchaseWidget> PurchaseWidgets;
            [Opt] public readonly EcsPool<PhysicView> PhysicView;
            [Opt] public readonly EcsPool<RenderCamera> RenderCamera;
            [Opt] public readonly EcsPool<Render3DToUIRequest> Render3DToUI;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out GameAspect gameAspect))
            {
                GameScreenAspect gameScreenAspect = _world.GetAspect<GameScreenAspect>();

                entlong gameScreenLong = _world.NewEntityLong();

                EcsEntityConnect connect = Object.Instantiate(gameAspect.GameScreenPrefabs.Read(entity).Value);

                connect.Connect(gameScreenLong, applyTemplates: true);

                ref GameScreen gameScreen = ref gameScreenAspect.GameScreens.Get(gameScreenLong.ID);

                gameScreen.Canvas.worldCamera = GameObject.FindWithTag("UICamera").GetComponent<Camera>();

                if (gameScreen.SettingsPopupConnect != null)
                    CreateSettingsPopup(in gameScreen);

                if (gameScreen.AnimalsShopWindowConnect != null)
                    CreateAnimalsShopWindow(in gameScreen);

                if (gameScreen.RewardWidgetConnect != null)
                    CreateReward(in gameScreen);

                if (gameScreen.GameTileWidgetConnect != null)
                    CreateGameTitle(in gameScreen);

                if (gameScreen.PlayWidgetConnect != null)
                    CreatePlayWidget(in gameScreen);

                if (gameScreen.ScoreWidgetConnect != null)
                    CreateScoreWidget(in gameScreen);

                if (gameScreen.CoinsWidgetConnect != null)
                    CreateCoinsWidget(in gameScreen);

                if (gameScreen.BestScoreWidgetConnect != null)
                    CreateBestScoreWidget(in gameScreen);                
                
                if (gameScreen.TutorialWindowConnect != null)
                    CreateTutorialWindow(in gameScreen);

                gameScreenAspect.GameScreenCreatedEvent.Add(gameScreenLong.ID);
            }
        }

        private void CreateTutorialWindow(in GameScreen gameScreen)
        {
            entlong window = _world.NewEntityLong();

            gameScreen.TutorialWindowConnect.Connect(window, applyTemplates: true);
        }

        private void CreateBestScoreWidget(in GameScreen gameScreen)
        {
            entlong widget = _world.NewEntityLong();

            gameScreen.BestScoreWidgetConnect.Connect(widget, applyTemplates: true);
        }

        private void CreateCoinsWidget(in GameScreen gameScreen)
        {
            entlong widget = _world.NewEntityLong();

            gameScreen.CoinsWidgetConnect.Connect(widget, applyTemplates: true);
        }

        private void CreateScoreWidget(in GameScreen gameScreen)
        {
            entlong widget = _world.NewEntityLong();

            gameScreen.ScoreWidgetConnect.Connect(widget, applyTemplates: true);
        }

        private void CreatePlayWidget(in GameScreen gameScreen)
        {
            entlong widget = _world.NewEntityLong();

            gameScreen.PlayWidgetConnect.Connect(widget, applyTemplates: true);
        }

        private void CreateSettingsPopup(in GameScreen gameScreen)
        {
            entlong popup = _world.NewEntityLong();

            gameScreen.SettingsPopupConnect.Connect(popup, applyTemplates: true);
        }

        private void CreateGameTitle(in GameScreen gameScreen)
        {
            entlong title = _world.NewEntityLong();

            GameTitleWidgetAspect widgetAspect = _world.GetAspect<GameTitleWidgetAspect>();

            gameScreen.GameTileWidgetConnect.Connect(title, applyTemplates: true);

            widgetAspect.Wobble.Add(title.ID);
        }

        private void CreateReward(in GameScreen gameScreen)
        {
            entlong reward = _world.NewEntityLong();

            gameScreen.RewardWidgetConnect.Connect(reward, applyTemplates: true);

            entlong rewardWindowLong = _world.NewEntityLong();

            RewardWidgetAspect rewardWidgetAspect = _world.GetAspect<RewardWidgetAspect>();

            ref RewardWidget rewardWidget = ref rewardWidgetAspect.RewardWidgets.Get(reward.ID);

            // create reward window
            rewardWidget.RewardWindowConnect.Connect(rewardWindowLong, applyTemplates: true);

            RewardWindowAspect rewardWindowAspect = _world.GetAspect<RewardWindowAspect>();

            // create coins reward
            ref readonly RewardWindow rewardWindow = ref rewardWindowAspect.RewardWindows.Read(rewardWindowLong.ID);
            rewardWindow.RewardCoinsWidgetConnect.Connect(_world.NewEntityLong(), applyTemplates: true);

            // congratulation
            rewardWindow.CongratulationWidgetConnect.Connect(_world.NewEntityLong(), applyTemplates: true);

            // tap to exit
            rewardWindow.TapToExitWidgetConnect.Connect(_world.NewEntityLong(), applyTemplates: true);

            // confetti
            rewardWindow.RewardConfettiEffectConnect.Connect(_world.NewEntityLong(), applyTemplates: true);
        }

        private void CreateAnimalsShopWindow(in GameScreen gameScreen)
        {
            entlong window = _world.NewEntityLong();

            gameScreen.AnimalsShopWindowConnect.Connect(window, applyTemplates: true);

            CreatePurchases(window.ID);
        }

        private void CreatePurchases(int window)
        {
            AnimalsShopWindowAspect windowAspect = _world.GetAspect<AnimalsShopWindowAspect>();

            ref Purchases purchases = ref windowAspect.Purchases.Get(window);

            purchases.Entities = EcsGroup.New(_world);

            ref ScrollSnap scrollSnap = ref windowAspect.ScrollSnaps.Get(window);

            scrollSnap.Items = purchases.Entities;

            windowAspect.ScrollSetupRequest.Add(window);

            foreach (int player in _world.Where(out PlayerAspect playerAspect))
            {
                ref readonly AnimalPrefabs animalPrefabs = ref playerAspect.AnimalPrefabs.Read(player);

                foreach (EcsEntityConnect purchasePrefab in purchases.Prefabs)
                {
                    EcsEntityConnect purchaseView = Object.Instantiate(
                        purchasePrefab,
                        scrollSnap.ScrollRect.content.transform,
                        worldPositionStays: false);

                    entlong purchaseLong = _world.NewEntityLong();
                    purchaseView.Connect(purchaseLong, applyTemplates: true);
                    purchases.Entities.Add(purchaseLong.ID);

                    ref readonly Purchase purchase = ref _world.GetPool<Purchase>().Read(purchaseLong.ID);
                    GameObject rendererPrefab = animalPrefabs.Animals[purchase.Index].Renderer;
                    GameObject renderer = Object.Instantiate(rendererPrefab);
                    PurchaseAspect purchaseAspect = _world.GetAspect<PurchaseAspect>();
                    purchaseAspect.PhysicView.Add(purchaseLong.ID).Value = renderer;

                    ref PurchaseWidget purchaseWidget = ref purchaseAspect.PurchaseWidgets.Get(purchaseLong.ID);

                    int renderer3D = _world.NewEntity();
                    purchaseAspect.Render3DToUI.Add(renderer3D) = new Render3DToUIRequest
                    {
                        RawImage = purchaseWidget.Icon
                    };
                    _world.GetPool<TargetEntity>().Add(renderer3D).Value = purchaseLong;

                    ref AnimalsShopWindow animalsShopWindow = ref windowAspect.AnimalsShopWindows.Get(window);
                    purchaseWidget.PurchaseStatusWidget = new PurchaseStatusWidget
                    {
                        Current = animalsShopWindow.PurchaseStatusWidget.Current,
                        Lock = animalsShopWindow.PurchaseStatusWidget.Lock,
                        Play = animalsShopWindow.PurchaseStatusWidget.Play,
                        Price = animalsShopWindow.PurchaseStatusWidget.Price,
                        PriceText = animalsShopWindow.PurchaseStatusWidget.PriceText,
                        Unlock = animalsShopWindow.PurchaseStatusWidget.Unlock
                    };
                }
            }
        }
    }
}