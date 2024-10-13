using System.Linq;
using _Project.Scripts.Gameplay.Features.AnimalFeature.Components;
using _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.Components;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.Components;
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.ButtonFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;
using YG;

namespace _Project.Scripts.Gameplay.Features.PlayerFeature.Systems
{
    public class SaveLoadPlayerProgressSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class PlayerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayerTag))]
            [Inc] public readonly EcsPool<Coins> Coins;

            [Inc] public readonly EcsPool<BestScore> BestScores;
            [Inc] public readonly EcsPool<SelectedAnimal> SelectedAnimals;
            [Inc] public readonly EcsPool<AnimalPrefabs> AnimalPrefabs;

            [Opt] public readonly EcsPool<CoinsUpdatedEvent> CoinsUpdated;

            [Opt] public readonly EcsPool<TargetEntity> TargetEntity;
            [Opt] public readonly EcsTagPool<PurchasesClearedEvent> PurchasesClearedEvent;
        }

        private class CoinUpdateEventAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CoinsUpdatedEvent))]
            [Inc] public readonly EcsPool<TargetEntity> TargetEntity;
        }

        private class BestScoreUpdatedEventAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(BestScoreUpdatedEvent))]
            [Inc] public readonly EcsPool<TargetEntity> TargetEntity;
        }

        private class LoadProgressAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<LoadProgressRequest> LoadProgressRequest;
        }

        private class PurchasedEventAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<PurchasedEvent> PurchasedEvent;
            [Inc] public readonly EcsTagPool<PurchaseTag> PurchaseAnimalTag;
        }

        private class PlayAnimalButtonClickedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<PlayAnimalButtonTag> PlayAnimalButtonTag;
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> Clicked;
        }

        private class PurchasesAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PurchaseTag))]
            [IncImplicit(typeof(ScrollSnappedMarker))]
            [Inc] public readonly EcsPool<PhysicView> PhysicViews;

            [Inc] public readonly EcsPool<Purchase> Purchases;

            [Opt] public readonly EcsTagPool<PurchasedMarker> Purchased;
        }

        private class AnimalsShopWindow : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalsShopWindowTag))]
            [Inc] public readonly EcsPool<Purchases> AnimalPurchases;

            [Inc] public readonly EcsPool<ScrollSnap> ScrollSnaps;
        }

        private class ResetProgressButtonClickedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> ButtonClicked;
            [Inc] public readonly EcsTagPool<ResetProgressButtonTag> ResetProgressButtonTag;
        }

        private class SelectedAnimalUpdatedEventAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(SelectedAnimalUpdatedEvent))]
            [Inc] public readonly EcsPool<TargetEntity> TargetEntity;
        }

        private class PurchasesClearedEventAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PurchasesClearedEvent))]
            [Inc] public readonly EcsPool<TargetEntity> TargetEntity;
        }

        private class RewardUpdatedEventAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(RewardUpdatedEvent))]
            [Inc] public readonly EcsPool<TargetEntity> TargetEntity;
        }

        private class RewardAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Reward> Rewards;
        }

        private class GameAudioUpdatedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<GameAudioUpdatedEvent> GameAudioUpdatedEvent;
            [Inc] public readonly EcsPool<AudioButtonsStatus> AudioButtonsStatus;
        }

        private class SettingsPopupAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(SettingsPopupTag))]
            [Inc] public readonly EcsPool<AudioButtonsStatus> AudioButtonsStatus;
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

            foreach (int @event in _world.Where(out BestScoreUpdatedEventAspect scoreUpdatedEventAspect))
            {
                PlayerAspect playerAspect = _world.GetAspect<PlayerAspect>();

                if (!scoreUpdatedEventAspect.TargetEntity.Read(@event).Value.TryGetID(out int targetID) ||
                    !playerAspect.IsMatches(targetID))
                    continue;

                ref BestScore bestScore = ref playerAspect.BestScores.Get(targetID);

                YandexGame.savesData.BestScores = bestScore.Value;

                YandexGame.SaveProgress();
            }

            foreach (int purchase in _world.Where(out PurchasedEventAspect _))
            {
                YandexGame.savesData.PurchasedAnimals.Add(_world.GetPool<Purchase>().Read(purchase).Index);

                YandexGame.SaveProgress();
            }

            foreach (int @event in _world.Where(out RewardUpdatedEventAspect rewardsCountUpdatedEventAspect))
            {
                RewardAspect rewardAspect = _world.GetAspect<RewardAspect>();

                if (!rewardsCountUpdatedEventAspect.TargetEntity.Read(@event).Value.TryGetID(out int targetID) ||
                    !rewardAspect.IsMatches(targetID))
                    continue;

                ref readonly Reward reward = ref rewardAspect.Rewards.Read(targetID);
                
                YandexGame.savesData.RewardCount = reward.ClaimedCount;
                YandexGame.savesData.RewardCollectedAt = reward.CollectionTime;

                YandexGame.SaveProgress();
            }

            foreach (int _ in _world.Where(out PlayAnimalButtonClickedAspect _))
            {
                foreach (int player in _world.Where(out PlayerAspect _))
                {
                    foreach (int animalPurchase in _world.Where(out PurchasesAspect purchasesAspect))
                    {
                        ref readonly Purchase purchase = ref purchasesAspect.Purchases.Read(animalPurchase);

                        ProgressUtils.UpdateSelectedAnimal(
                            target: player,
                            selectedID: purchase.Index);
                    }
                }
            }

            foreach (int @event in _world.Where(out SelectedAnimalUpdatedEventAspect selectedAnimalUpdatedEventAspect))
            {
                PlayerAspect playerAspect = _world.GetAspect<PlayerAspect>();

                if (!selectedAnimalUpdatedEventAspect.TargetEntity.Read(@event).Value.TryGetID(out int targetID) ||
                    !playerAspect.IsMatches(targetID))
                    continue;

                ref SelectedAnimal selectedAnimal = ref playerAspect.SelectedAnimals.Get(targetID);

                YandexGame.savesData.SelectedAnimalID = selectedAnimal.ID;

                YandexGame.SaveProgress();
            }

            foreach (int @event in _world.Where(out PurchasesClearedEventAspect purchasesClearedEventAspect))
            {
                PlayerAspect playerAspect = _world.GetAspect<PlayerAspect>();

                if (!purchasesClearedEventAspect.TargetEntity.Read(@event).Value.TryGetID(out int targetID) ||
                    !playerAspect.IsMatches(targetID))
                    continue;

                YandexGame.savesData.PurchasedAnimals.Clear();

                YandexGame.SaveProgress();
            }

            foreach (int entity in _world.Where(out GameAudioUpdatedAspect aspect))
            {
                ref readonly AudioButtonsStatus status = ref aspect.AudioButtonsStatus.Read(entity);

                YandexGame.savesData.Audio.MusicIsOn = status.MusicIsOn;
                YandexGame.savesData.Audio.SoundIsOn = status.SoundIsOn;
                
                YandexGame.SaveProgress();
            }

            foreach (int _ in _world.Where(out ResetProgressButtonClickedAspect _))
            {
                foreach (int player in _world.Where(out PlayerAspect playerAspect))
                {
                    int @event = _world.NewEntity();
                    playerAspect.PurchasesClearedEvent.Add(@event);

                    ProgressUtils.UpdateCoins(
                        target: player,
                        coins: 0,
                        overwrite: true);

                    ProgressUtils.UpdateBestScore(
                        target: player,
                        scores: 0,
                        overwrite: true);

                    ProgressUtils.UpdateSelectedAnimal(
                        target: player,
                        selectedID: 0);

                    ProgressUtils.ClearPurchases(target: player);

                    YandexGame.SaveProgress();
                }

                foreach (int reward in _world.Where(out RewardAspect _))
                {
                    ProgressUtils.UpdateReward(
                        target: reward,
                        time: 0,
                        count: 0,
                        overwrite: true);
                }
            }

            foreach (int _ in _world.Where(out LoadProgressAspect _))
            {
                foreach (int player in _world.Where(out PlayerAspect _))
                {
                    ProgressUtils.UpdateCoins(
                        target: player,
                        coins: YandexGame.savesData.Coins,
                        overwrite: true);

                    ProgressUtils.UpdateBestScore(
                        target: player,
                        scores: YandexGame.savesData.BestScores,
                        overwrite: true);

                    ProgressUtils.UpdateSelectedAnimal(
                        target: player,
                        selectedID: YandexGame.savesData.SelectedAnimalID);
                }

                foreach (int window in _world.Where(out AnimalsShopWindow aspect))
                {
                    Debug.Log("Load Purchases");

                    ref Purchases purchases = ref aspect.AnimalPurchases.Get(window);

                    foreach (int purchased in purchases.Entities.Where(x =>
                                 YandexGame.savesData.PurchasedAnimals.Contains(_world.GetPool<Purchase>().Read(x)
                                     .Index)))
                    {
                        _world.GetPool<PurchasedMarker>().Add(purchased);
                    }

                    ref ScrollSnap scrollSnap = ref aspect.ScrollSnaps.Get(window);

                    scrollSnap.TargetIndex = YandexGame.savesData.SelectedAnimalID;
                }

                foreach (int reward in _world.Where(out RewardAspect _))
                {
                    ProgressUtils.UpdateReward(
                        target: reward,
                        time: YandexGame.savesData.RewardCollectedAt,
                        count: YandexGame.savesData.RewardCount);
                }

                foreach (int entity in _world.Where(out SettingsPopupAspect _))
                {
                    ProgressUtils.UpdateGameAudio(
                        target: entity, 
                        isMusicOn: YandexGame.savesData.Audio.MusicIsOn,
                        isSoundOn: YandexGame.savesData.Audio.SoundIsOn);
                }
            }
        }
    }
}