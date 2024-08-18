using System;
using System.Linq;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Systems;
using DCFApixels.DragonECS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class GameScreenSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameCreatedEvent))]
            [Inc] public readonly EcsPool<GameScreenCfg> GameScreenConfigs;

            [Opt] public readonly EcsPool<GameScreen> GameScreen;
        }

        private class GameScreenAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Prefab> Prefabs;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                entlong screen = _world.NewEntityLong(aspect.GameScreenConfigs.Read(entity).Value);

                GameScreenAspect screenAspect = _world.GetAspect<GameScreenAspect>();

                if (screenAspect.IsMatches(screen.ID))
                {
                    EcsEntityConnect connect = Object.Instantiate(screenAspect.Prefabs.Read(screen.ID).Value);

                    connect.Connect(screen, false);

                    foreach (MonoEntityTemplateBase template in connect.MonoTemplates)
                        template.Apply(_world.id, screen.ID);

                    aspect.GameScreen.TryAddOrGet(entity).Value = screen;

                    CreateBest(connect);
                    CreateInAppPurchases(connect);
                    CreateAnimalsShop(connect);
                }
            }
        }

        private void CreateAnimalsShop(EcsEntityConnect connect)
        {
            entlong screen = connect.Entity;

            int shop = _world.NewEntity();

            _world.GetPool<AnimalStoreView>().Read(screen.ID).Value.Connect(shop.ToEntityLong(_world), true);

            ref InGamePurchaseAnimals inGamePurchaseAnimals =
                ref _world.GetPool<InGamePurchaseAnimals>().Get(shop);

            ref ScrollSnapRef scrollSnap = ref _world.GetPool<ScrollSnapRef>().Get(shop);

            inGamePurchaseAnimals.Value = EcsGroup.New(_world);
            
            RectTransform content = scrollSnap.Value.GetComponent<ScrollRect>().content;

            TextMeshProUGUI priceText = scrollSnap.Value.transform.Find("Viewport/Price/Price").GetComponent<TextMeshProUGUI>();

            Debug.Log(priceText);
            
            foreach (GameObject animalPrefab in inGamePurchaseAnimals.Proto)
            {
                ModelToUIRenderer purchaseRenderer = Object
                    .Instantiate(inGamePurchaseAnimals.Purchase, content.transform, false)
                    .GetComponent<ModelToUIRenderer>();

                Transform renderer = animalPrefab.transform.Find("Renderer");

                purchaseRenderer.modelPrefab = renderer.gameObject;

                EcsEntityConnect animalConnect = purchaseRenderer.GetComponent<EcsEntityConnect>();

                entlong purchase = _world.NewEntityLong();

                animalConnect.Connect(purchase, true);

                inGamePurchaseAnimals.Value.Add(purchase.ID);
                
                var view = purchaseRenderer.Create();

                _world.GetPool<PhysicView>().Add(purchase.ID).Value = view;
                
                _world.GetPool<CreatedEvent>().Add(purchase.ID);
                
                _world.GetPool<TextMeshProUGUIRef>().Get(purchase.ID).Value = priceText;
                
                _world.GetPool<Purchase>().Get(purchase.ID).Price += inGamePurchaseAnimals.Proto.ToList().IndexOf(animalPrefab);
            }
        }

        private void CreateInAppPurchases(EcsEntityConnect connect)
        {
            entlong screen = connect.Entity;

            Transform shop = connect.transform.GetChild(0).Find("InAppShop").Find("Products");

            ref readonly InAppPurchases inAppPurchases = ref _world.GetPool<InAppPurchases>().Read(screen.ID);

            foreach (ref readonly InAppPurchases.Purchase purchase in inAppPurchases.Value.AsSpan())
                Object.Instantiate(purchase.Prefab, shop.transform, false);
        }

        private void CreateBest(EcsEntityConnect connect)
        {
            entlong screen = _world.NewEntityLong();

            // TODO: 
            EcsEntityConnect bestConnect = connect.transform.GetChild(0).Find("Best").GetComponent<EcsEntityConnect>();

            bestConnect.Connect(screen, true);
        }
    }
}