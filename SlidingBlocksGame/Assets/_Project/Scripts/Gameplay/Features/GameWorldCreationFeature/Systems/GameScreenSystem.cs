using System;
using System.Collections.Generic;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.ScrollFeature;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Systems;
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
            [IncImplicit(typeof(GameCreatedEvent))]
            [Inc] public readonly EcsPool<GameScreenPrefab> GameScreenPrefabs;

            [Opt] public readonly EcsPool<GameScreen> GameScreen;
        }

        private class GameAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<AnimalPrefabs> AnimalPrefabs;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect gameAspect))
            {
                entlong gameScreen = _world.NewEntityLong();
                
                EcsEntityConnect connect = Object.Instantiate(gameAspect.GameScreenPrefabs.Read(entity).Value);

                connect.Connect(gameScreen, applyTemplates: true);

                gameAspect.GameScreen.Add(entity).Value = gameScreen;

                CreateInAppPurchases(connect);
                CreateAnimalsPurchaseWindow(connect);
            }
        }

        private void CreateAnimalsPurchaseWindow(EcsEntityConnect connect)
        {
            entlong screen = connect.Entity;

            int shop = _world.NewEntity();

            _world.GetPool<ClosedMarker>().Add(shop);

            AnimalStoreView animalStoreView = _world.GetPool<AnimalStoreView>().Read(screen.ID);
            animalStoreView.Value.Connect(shop.ToEntityLong(_world), true);

            ref AnimalPurchases animalPurchases =
                ref _world.GetPool<AnimalPurchases>().Get(shop);

            animalPurchases.Entities = EcsGroup.New(_world);

            RectTransform content = animalStoreView.Value.GetComponent<ScrollRect>().content;

            TextMeshProUGUI priceText =
                animalStoreView.Value.transform.Find("Viewport/Price/Price").GetComponent<TextMeshProUGUI>();
            
            _world.GetPool<ScrollSnap>().Get(animalStoreView.Value.Entity.ID).Items = animalPurchases.Entities;
            _world.GetPool<ScrollSetupRequest>().Add(animalStoreView.Value.Entity.ID);

            List<GameObject> list = new List<GameObject>();

            foreach (int game in _world.Where(out GameAspect gameAspect))
            {
                ref readonly AnimalPrefabs animalPrefabs = ref gameAspect.AnimalPrefabs.Read(game);

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

            _world.GetPool<SnappedMarker>().Add(animalPurchases.Entities[0]);
        }

        private void UI3D(AnimalPrefabs animalPrefabs, entlong purchase, EcsEntityConnect purchaseView)
        {
            FitObjectToOrthographicCamera fitObjectToOrthographicCamera =
                purchaseView.GetComponent<FitObjectToOrthographicCamera>();

            GameObject animalRenderer = animalPrefabs.Animals[_world.GetPool<Purchase>().Read(purchase.ID).ProductIndex]
                .Renderer;

            GameObject animalRendererView = Object.Instantiate(animalRenderer);

            _world.GetPool<PhysicView>().Add(purchase.ID).Value = animalRendererView;

            _world.GetPool<SpawnedEvent>().Add(purchase.ID);

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