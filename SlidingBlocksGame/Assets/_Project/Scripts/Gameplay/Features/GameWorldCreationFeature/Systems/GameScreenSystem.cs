using System;
using System.Collections.Generic;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.ScrollFeature;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Systems;
using _Project.Scripts.Gameplay.Features.VisualFeature.Components;
using DCFApixels.DragonECS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using ScrollSnap = _Project.Scripts.Gameplay.Features.UIFeature.Components.ScrollSnap;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class GameScreenSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CreateGameScreenRequest))]
            [Inc] public readonly EcsPool<GameScreenPrefab> GameScreenPrefabs;

            [Opt] public readonly EcsPool<GameScreen> GameScreen;
        }

        private class PlayerAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<AnimalPrefabs> AnimalPrefabs;
        }

        private class GameScreenAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsTagPool<WobbleRequest> Wobble;
            [Inc] public readonly EcsPool<CanvasRef> Canvases;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect gameAspect))
            {
                GameScreenAspect gameScreenAspect = _world.GetAspect<GameScreenAspect>();

                entlong gameScreen = _world.NewEntityLong();

                EcsEntityConnect connect = Object.Instantiate(gameAspect.GameScreenPrefabs.Read(entity).Value);

                connect.Connect(gameScreen, applyTemplates: true);

                gameAspect.GameScreen.Add(entity).Value = gameScreen;

                Camera uiCamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();

                gameScreenAspect.Canvases.Get(gameScreen.ID).Value.worldCamera = uiCamera;

                CreateInAppPurchases(connect);
                CreateAnimalsPurchaseWindow(connect);
                CreateReward(connect);
                CreateGameTitle(connect, gameScreenAspect);
                CreateSettingsPopup(connect, gameScreenAspect);


                _world.GetPool<GameScreenCreatedEvent>().Add(gameScreen.ID);
            }
        }

        private void CreateSettingsPopup(EcsEntityConnect connect, GameScreenAspect gameScreenAspect)
        {
            entlong screen = connect.Entity;

            entlong popup = _world.NewEntityLong();

            _world.GetPool<SettingsPopupConnect>().Get(screen.ID).Value
                .Connect(popup, applyTemplates: true);
        }

        private void CreateGameTitle(EcsEntityConnect connect, GameScreenAspect gameScreenAspect)
        {
            entlong screen = connect.Entity;

            entlong title = _world.NewEntityLong();

            _world.GetPool<GameTitleConnect>().Get(screen.ID).Value
                .Connect(title, applyTemplates: true);

            gameScreenAspect.Wobble.Add(title.ID);
        }

        private void CreateReward(EcsEntityConnect connect)
        {
            entlong screen = connect.Entity;

            entlong reward = _world.NewEntityLong();

            _world.GetPool<RewardConnect>().Get(screen.ID).Value.Connect(reward, applyTemplates: true);

            // create reward window
            entlong rewardWindow = _world.NewEntityLong();

            ref RewardWindowConnect rewardWindowConnect = ref _world.GetPool<RewardWindowConnect>().Get(reward.ID);

            rewardWindowConnect.Value.Connect(rewardWindow, applyTemplates: true);

            // create coins reward
            _world.GetPool<CoinsRewardConnect>().Get(rewardWindow.ID).Value
                .Connect(_world.NewEntityLong(), applyTemplates: true);

            // congratulation
            _world.GetPool<CongratulationConnect>().Get(rewardWindow.ID).Value
                .Connect(_world.NewEntityLong(), applyTemplates: true);

            // tap to exit
            _world.GetPool<TapToExitConnect>().Get(rewardWindow.ID).Value
                .Connect(_world.NewEntityLong(), applyTemplates: true);

            // confetti
            _world.GetPool<RewardConfettiEffectConnect>().Get(rewardWindow.ID).Value
                .Connect(_world.NewEntityLong(), applyTemplates: true);
        }

        private void CreateAnimalsPurchaseWindow(EcsEntityConnect connect)
        {
            entlong screen = connect.Entity;

            int shop = _world.NewEntity();

            _world.GetPool<ClosedMarker>().Add(shop);

            AnimalsShopWindowConnect animalsShopWindowConnect =
                _world.GetPool<AnimalsShopWindowConnect>().Read(screen.ID);
            animalsShopWindowConnect.Value.Connect(shop.ToEntityLong(_world), true);

            ref AnimalPurchases animalPurchases =
                ref _world.GetPool<AnimalPurchases>().Get(shop);

            animalPurchases.Entities = EcsGroup.New(_world);

            RectTransform content = animalsShopWindowConnect.Value.GetComponent<ScrollRect>().content;

            TextMeshProUGUI priceText =
                animalsShopWindowConnect.Value.transform.Find("Viewport/Price/Price").GetComponent<TextMeshProUGUI>();

            _world.GetPool<ScrollSnap>().Get(animalsShopWindowConnect.Value.Entity.ID).Items = animalPurchases.Entities;
            ref ScrollSetupRequest scrollSetupRequest =
                ref _world.GetPool<ScrollSetupRequest>().Add(animalsShopWindowConnect.Value.Entity.ID);

            List<GameObject> list = new List<GameObject>();

            foreach (int player in _world.Where(out PlayerAspect playerAspect))
            {
                ref readonly AnimalPrefabs animalPrefabs = ref playerAspect.AnimalPrefabs.Read(player);

                foreach (EcsEntityConnect purchasePrefab in animalPurchases.Prefabs)
                {
                    EcsEntityConnect purchaseView = Object
                        .Instantiate(purchasePrefab, content.transform, false);

                    list.Add(purchaseView.gameObject);

                    entlong purchase = _world.NewEntityLong();

                    purchaseView.Connect(purchase, true);

                    animalPurchases.Entities.Add(purchase.ID);

                    UI3D(animalPrefabs, purchase, purchaseView);

                    _world.GetPool<TextMeshProUGUIRef>().Get(purchase.ID).Value = priceText;

                    _world.GetPool<ScrollPosition>().Add(purchase.ID);
                }
            }
        }

        private void UI3D(AnimalPrefabs animalPrefabs, entlong purchase, EcsEntityConnect purchaseView)
        {
            FitObjectToOrthographicCamera fitObjectToOrthographicCamera =
                purchaseView.GetComponent<FitObjectToOrthographicCamera>();

            GameObject animalRenderer = animalPrefabs.Animals[_world.GetPool<Purchase>().Read(purchase.ID).ProductIndex]
                .Renderer;

            GameObject animalRendererView = Object.Instantiate(animalRenderer);

            _world.GetPool<PhysicView>().Add(purchase.ID).Value = animalRendererView;

            var cam = fitObjectToOrthographicCamera.Create(animalRendererView);

            _world.GetPool<RenderCamera>().Add(purchase.ID).Value = cam;
        }

        private void CreateInAppPurchases(EcsEntityConnect connect)
        {
            entlong screen = connect.Entity;

            Transform shop = connect.transform.GetChild(0).Find("InAppShop").Find("Products");

            ref readonly InAppPurchases inAppPurchases = ref _world.GetPool<InAppPurchases>().Read(screen.ID);

            foreach (ref readonly InAppPurchases.Purchase purchase in inAppPurchases.Value.AsSpan())
                Object.Instantiate(purchase.Prefab, shop.transform, false);
        }
    }
}