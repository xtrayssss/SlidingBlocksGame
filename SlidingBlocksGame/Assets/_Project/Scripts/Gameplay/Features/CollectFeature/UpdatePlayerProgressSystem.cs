using System.Linq;
using _Project.Scripts.Gameplay.Features.CollectFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;
using YG;

namespace _Project.Scripts.Gameplay.Features.CollectFeature
{
    public class UpdatePlayerProgressSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class PlayerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayerTag))]
            [Inc] public readonly EcsPool<Coins> Coins;

            [Inc] public readonly EcsPool<Scores> Scores;
            [Inc] public readonly EcsPool<SelectedAnimal> SelectedAnimals;
            [Inc] public readonly EcsPool<AnimalPrefabs> AnimalPrefabs;

            [Opt] public readonly EcsTagPool<CoinsUpdatedEvent> CoinsUpdated;
            [Opt] public readonly EcsTagPool<ScoresUpdatedEvent> ScoresUpdated;
            [Opt] public readonly EcsPool<TargetEntity> TargetEntity;
            [Opt] public readonly EcsTagPool<ClearPurchasesRequest> ClearPurchases;
        }

        private class CoinUpdateEventAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CoinsUpdatedEvent))]
            [Inc] public readonly EcsPool<TargetEntity> TargetEntity;
        }

        private class ScoreUpdatedEventAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ScoresUpdatedEvent))]
            [Inc] public readonly EcsPool<TargetEntity> TargetEntity;
        }

        private class GameCreatedAspect : EcsAspectAuto
        {
            [Inc] private readonly EcsTagPool<GameCreatedEvent> _gameCreatedEvents;
        }

        private class PurchasedEventAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<PurchasedEvent> PurchasedEvent;
            [Inc] public readonly EcsPool<PurchasedEntity> PurchasedEntities;
        }

        private class PlayAnimalButtonClickedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<PlayAnimalButtonTag> PlayAnimalButtonTag;
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> Clicked;
        }

        private class PurchasesAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PurchaseAnimalTag))]
            [IncImplicit(typeof(SnappedMarker))]
            [Inc] public readonly EcsPool<PhysicView> PhysicViews;

            [Inc] public readonly EcsPool<Purchase> Purchases;

            [Opt] public readonly EcsTagPool<PurchasedMarker> Purchased;
        }

        private class AnimalsShopWindow : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalsShopWindowTag))]
            [IncImplicit(typeof(AnimalsShopWindowCreatedEvent))]
            [Inc] public readonly EcsPool<AnimalPurchases> AnimalPurchases;
        }

        private class RewardedEventAspect : EcsAspectAuto
        {
            [Inc] private readonly EcsTagPool<RewardedEvent> _rewardedEvent;
        }

        private class ResetProgressButtonClickedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> ButtonClicked;
            [Inc] public readonly EcsTagPool<ResetProgressButtonTag> ResetProgressButtonTag;
        }

        public void Run()
        {
            foreach (int @event in _world.Where(out CoinUpdateEventAspect coinCollectedAspect))
            {
                PlayerAspect playerAspect = _world.GetAspect<PlayerAspect>();

                if (!coinCollectedAspect.TargetEntity.Read(@event).Value.TryGetID(out int targetID) ||
                    !playerAspect.IsMatches(targetID))
                    continue;

                ref Coins coins = ref playerAspect.Coins.Get(targetID);

                YandexGame.savesData.Coins = coins.Value;

                YandexGame.SaveProgress();
            }

            foreach (int @event in _world.Where(out ScoreUpdatedEventAspect scoreUpdatedEventAspect))
            {
                PlayerAspect playerAspect = _world.GetAspect<PlayerAspect>();

                if (!scoreUpdatedEventAspect.TargetEntity.Read(@event).Value.TryGetID(out int targetID) ||
                    !playerAspect.IsMatches(targetID))
                    continue;

                ref Scores scores = ref playerAspect.Scores.Get(targetID);

                YandexGame.savesData.Scores = scores.Value;

                YandexGame.SaveProgress();
            }

            foreach (int @event in _world.Where(out PurchasedEventAspect purchasedEventAspect))
            {
                if (!purchasedEventAspect.PurchasedEntities.Read(@event).Value.TryGetID(out int purchaseID))
                    continue;

                YandexGame.savesData.PurchasedAnimals.Add(_world.GetPool<Purchase>().Read(purchaseID).ProductIndex);

                YandexGame.SaveProgress();
            }

            foreach (int _ in _world.Where(out RewardedEventAspect _))
            {
                foreach (int player in _world.Where(out PlayerAspect playerAspect))
                {
                    ref Coins playerCoins = ref playerAspect.Coins.Get(player);

                    YandexGame.savesData.RewardCount++;

                    YandexGame.savesData.Coins = playerCoins.Value;

                    YandexGame.SaveProgress();
                }
            }

            foreach (int _ in _world.Where(out PlayAnimalButtonClickedAspect _))
            {
                foreach (int player in _world.Where(out PlayerAspect playerAspect))
                {
                    foreach (int animalPurchase in _world.Where(out PurchasesAspect purchasesAspect))
                    {
                        ref readonly Purchase purchase = ref purchasesAspect.Purchases.Read(animalPurchase);

                        ref var selectedAnimal = ref playerAspect.SelectedAnimals.Get(player);
                        selectedAnimal.ID = purchase.ProductIndex;
                        selectedAnimal.Prefab = playerAspect.AnimalPrefabs.Read(player).Animals[selectedAnimal.ID];

                        YandexGame.savesData.SelectedAnimalID = purchase.ProductIndex;

                        YandexGame.SaveProgress();
                    }
                }
            }

            foreach (int _ in _world.Where(out ResetProgressButtonClickedAspect _))
            {
                foreach (int player in _world.Where(out PlayerAspect playerAspect))
                {
                    playerAspect.Coins.Get(player).Value = 0;
                    playerAspect.Scores.Get(player).Value = 0;

                    ref SelectedAnimal selectedAnimal = ref playerAspect.SelectedAnimals.Get(player);
                    selectedAnimal.ID = 0;
                    selectedAnimal.Prefab = playerAspect.AnimalPrefabs.Read(player).Animals[selectedAnimal.ID];

                    SendCoinsEvent();
                    SendScoresEvent();
                    SendClearPurchasesEvent();

                    void SendCoinsEvent()
                    {
                        int @event = _world.NewEntity();
                        playerAspect.CoinsUpdated.Add(@event);
                        playerAspect.TargetEntity.Add(@event).Value = player.ToEntityLong(_world);
                    }

                    void SendScoresEvent()
                    {
                        int @event = _world.NewEntity();
                        playerAspect.ScoresUpdated.Add(@event);
                        playerAspect.TargetEntity.Add(@event).Value = player.ToEntityLong(_world);
                    }

                    void SendClearPurchasesEvent()
                    {
                        int @event = _world.NewEntity();
                        playerAspect.ClearPurchases.Add(@event);
                    }

                    YandexGame.savesData.Coins = 0;
                    YandexGame.savesData.Scores = 0;
                    YandexGame.savesData.SelectedAnimalID = 0;
                    YandexGame.savesData.PurchasedAnimals.Clear();
                    YandexGame.savesData.RewardCount = 0;

                    YandexGame.SaveProgress();
                }
            }

            // primary
            foreach (int _ in _world.Where(out GameCreatedAspect _))
            {
                foreach (int player in _world.Where(out PlayerAspect playerAspect))
                {
                    playerAspect.Coins.Get(player).Value = YandexGame.savesData.Coins;
                    playerAspect.Scores.Get(player).Value = YandexGame.savesData.Scores;

                    ref SelectedAnimal selectedAnimal = ref playerAspect.SelectedAnimals.Get(player);
                    selectedAnimal.ID = YandexGame.savesData.SelectedAnimalID;
                    selectedAnimal.Prefab = playerAspect.AnimalPrefabs.Read(player).Animals[selectedAnimal.ID];
                }
            }

            foreach (int window in _world.Where(out AnimalsShopWindow aspect))
            {
                Debug.Log("Load Purchases");

                ref AnimalPurchases animalPurchases = ref aspect.AnimalPurchases.Get(window);

                foreach (int purchased in animalPurchases.Entities.Where(x =>
                             YandexGame.savesData.PurchasedAnimals.Contains(_world.GetPool<Purchase>().Read(x)
                                 .ProductIndex)))
                {
                    _world.GetPool<PurchasedMarker>().Add(purchased);
                }
            }
        }
    }
}