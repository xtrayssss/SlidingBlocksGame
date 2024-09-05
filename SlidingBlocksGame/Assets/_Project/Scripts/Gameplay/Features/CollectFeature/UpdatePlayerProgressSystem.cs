using System.Linq;
using _Project.Scripts.Gameplay.Features.CollectFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
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
        }

        private class CoinCollectedAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CoinCollectedEvent))]
            [Inc] public readonly EcsPool<Coins> Coins;
        }

        private class ScoreUpdatedAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelTag))]
            [IncImplicit(typeof(SpawnedEvent))]
            [Inc] public readonly EcsPool<Scores> Scores;
        }

        private class GameCreatedAspect : EcsAspectAuto
        {
            [Inc] private readonly EcsTagPool<GameCreatedEvent> _gameCreatedEvents;
        }

        private class PurchaseButtonClickedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<UnlockButtonTag> UnlockButtonTag;
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

        private class AnimalPurchasesWindowSpawnedAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalsShopWindowTag))]
            [IncImplicit(typeof(SpawnedEvent))]
            [Inc] public readonly EcsPool<AnimalPurchases> AnimalPurchases;
        }

        private class RewardButtonClickedAspect : EcsAspectAuto
        {
            [Inc] private readonly EcsTagPool<RewardButtonTag> _rewardButtonTag;
            [Inc] private readonly EcsTagPool<ButtonClickedEvent> _buttonClickedEvent;
        }

        private class RewardAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(RewardTag))]
            [IncImplicit(typeof(CanRewardMarker))]
            [Inc] public readonly EcsPool<CoinsProgressionCurve> CoinsProgressionCurves;
        }

        public void Run()
        {
            foreach (int @event in _world.Where(out CoinCollectedAspect coinCollectedAspect))
            {
                foreach (int entity in _world.Where(out PlayerAspect playerAspect))
                {
                    ref Coins coins = ref playerAspect.Coins.Get(entity);
                    coins.Value += coinCollectedAspect.Coins.Read(@event).Value;

                    YandexGame.savesData.Coins = coins.Value;

                    YandexGame.SaveProgress();
                }
            }

            foreach (int @event in _world.Where(out ScoreUpdatedAspect scoreUpdateAspect))
            {
                foreach (int entity in _world.Where(out PlayerAspect playerAspect))
                {
                    ref Scores scores = ref playerAspect.Scores.Get(entity);

                    scores.Value += scoreUpdateAspect.Scores.Read(@event).Value;

                    YandexGame.savesData.Scores = scores.Value;

                    YandexGame.SaveProgress();
                }
            }

            foreach (int _ in _world.Where(out PurchaseButtonClickedAspect _))
            {
                foreach (int player in _world.Where(out PlayerAspect playerAspect))
                {
                    foreach (int animalPurchase in _world.Where(out PurchasesAspect purchasesAspect))
                    {
                        ref Coins coins = ref playerAspect.Coins.Get(player);

                        ref readonly Purchase purchase = ref purchasesAspect.Purchases.Read(animalPurchase);

                        coins.Value -= purchase.Price;

                        purchasesAspect.Purchased.Add(animalPurchase);

                        YandexGame.savesData.Coins = coins.Value;

                        YandexGame.savesData.PurchasedAnimals.Add(purchase.ProductIndex);

                        YandexGame.SaveProgress();
                    }
                }
            }

            foreach (int _ in _world.Where(out RewardButtonClickedAspect _))
            {
                foreach (int reward in _world.Where(out RewardAspect aspect))
                {
                    foreach (int player in _world.Where(out PlayerAspect playerAspect))
                    {
                        ref Coins playerCoins = ref playerAspect.Coins.Get(player);

                        YandexGame.savesData.RewardCollectedAt = YandexGame.ServerTime();

                        int rewardCoins = (int)aspect.CoinsProgressionCurves.Read(reward).Value
                            .Evaluate(YandexGame.savesData.RewardCount);

                        playerCoins.Value += rewardCoins;

                        YandexGame.savesData.RewardCount++;

                        YandexGame.savesData.Coins = playerCoins.Value;

                        YandexGame.SaveProgress();

                        Debug.Log("Result");
                    }
                }
            }

            // primary
            foreach (int _ in _world.Where(out GameCreatedAspect _))
            {
                foreach (int player in _world.Where(out PlayerAspect playerAspect))
                {
                    playerAspect.Coins.Get(player).Value = YandexGame.savesData.Coins;
                    playerAspect.Scores.Get(player).Value = YandexGame.savesData.Scores;
                }
            }

            foreach (int window in _world.Where(out AnimalPurchasesWindowSpawnedAspect aspect))
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