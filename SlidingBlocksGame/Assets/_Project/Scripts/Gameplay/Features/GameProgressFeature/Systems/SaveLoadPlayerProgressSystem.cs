using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Gameplay.Features.AnimalFeature.Components;
using _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.CoinFeature.Components;
using _Project.Scripts.Gameplay.Features.CoinFeature.Utils;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameAudioFeature.Components;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.Utils;
using _Project.Scripts.Gameplay.Features.ScoreFeature.Components;
using _Project.Scripts.Gameplay.Features.ScoreFeature.Utils;
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.ButtonFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;
using YG;
using AudioSettings = _Project.Scripts.Gameplay.Features.GameAudioFeature.Components.AudioSettings;

namespace _Project.Scripts.Gameplay.Features.GameProgressFeature.Systems
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

            [Opt] public readonly EcsPool<TargetEntity> TargetEntity;
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
            [IncImplicit(typeof(SnappedState))]
            [Inc] public readonly EcsPool<PhysicView> PhysicViews;

            [Inc] public readonly EcsPool<Purchase> Purchases;

            [Opt] public readonly EcsTagPool<PurchasedMarker> Purchased;
        }

        private class AnimalsShopWindowAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalsShopWindowTag))]
            [Inc] public readonly EcsPool<Purchases> AnimalPurchases;

            [Inc] public readonly EcsPool<ScrollSnap> ScrollSnaps;
            [Opt] public readonly EcsPool<SetupScrollRequest> SetupScroll;
        }

        private class RewardUpdatedEventAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(RewardUpdatedEvent))]
            [Inc] public readonly EcsPool<TargetEntity> Rewards;
        }

        private class RewardAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Reward> Rewards;
        }

        private class AudioSettingsUpdatedEventAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<AudioSettingsUpdatedEvent> AudioSettingsUpdatedEvent;
        }

        private class SettingsAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<AudioSettings> AudioSettings;
            [Opt] public readonly EcsTagPool<AudioSettingsUpdatedEvent> GameAudioUpdatedEvent;
        }

        private class UpdateAudioSettingsRequestAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsPool<UpdateAudioSettingsRequest> UpdateAudioSettings;
        }

        public void Run()
        {
            foreach (int @event in _world.Where(out CoinUpdateEventAspect eventAspect))
            {
                PlayerAspect playerAspect = _world.GetAspect<PlayerAspect>();

                if (!eventAspect.TargetEntity.Read(@event).Value.TryGetID(out int playerID) ||
                    !playerAspect.IsMatches(playerID))
                    continue;

                ref Coins coins = ref playerAspect.Coins.Get(playerID);

                YandexGame.savesData.Coins = coins.Value;

                YandexGame.SaveProgress();
            }

            foreach (int @event in _world.Where(out BestScoreUpdatedEventAspect eventAspect))
            {
                PlayerAspect playerAspect = _world.GetAspect<PlayerAspect>();

                if (!eventAspect.TargetEntity.Read(@event).Value.TryGetID(out int playerID) ||
                    !playerAspect.IsMatches(playerID))
                    continue;

                ref BestScore bestScore = ref playerAspect.BestScores.Get(playerID);

                YandexGame.savesData.BestScores = bestScore.Value;

                YandexGame.SaveProgress();
            }

            foreach (int purchaseID in _world.Where(out PurchasedEventAspect _))
            {
                ref readonly Purchase purchase = ref _world.GetPool<Purchase>().Read(purchaseID);
                YandexGame.savesData.PurchasedAnimals.Add(purchase.Index);
                YandexGame.SaveProgress();
            }

            foreach (int _ in _world.Where(out AudioSettingsUpdatedEventAspect _))
            {
                foreach (int settings in _world.Where(out SettingsAspect settingsAspect))
                {
                    ref AudioSettings audioSettings = ref settingsAspect.AudioSettings.Get(settings);

                    YandexGame.savesData.Audio.MusicIsOn = audioSettings.MusicIsOn;
                    YandexGame.savesData.Audio.SoundIsOn = audioSettings.SoundIsOn;

                    YandexGame.SaveProgress();
                }
            }

            foreach (int @event in _world.Where(out RewardUpdatedEventAspect eventAspect))
            {
                RewardAspect rewardAspect = _world.GetAspect<RewardAspect>();

                if (!eventAspect.Rewards.Read(@event).Value.TryGetID(out int rewardID))
                    continue;

                ref readonly Reward reward = ref rewardAspect.Rewards.Read(rewardID);

                YandexGame.savesData.RewardCount = reward.ClaimedCount;
                YandexGame.savesData.RewardCollectionTime = reward.CollectionTime;

                YandexGame.SaveProgress();
            }

            foreach (int _ in _world.Where(out PlayAnimalButtonClickedAspect _))
            {
                foreach (int animalPurchase in _world.Where(out PurchasesAspect purchasesAspect))
                {
                    ref readonly Purchase purchase = ref purchasesAspect.Purchases.Read(animalPurchase);
                    UpdateSelectedAnimal(selectedID: purchase.Index);
                }
            }

            foreach (int _ in _world.Where(out LoadProgressAspect _))
            {
                foreach (int player in _world.Where(out PlayerAspect _))
                {
                    CoinUtils.Update(
                        coinable: player,
                        coins: YandexGame.savesData.Coins,
                        overwrite: true);

                    ScoreUtils.UpdateBestScore(
                        scorable: player,
                        score: YandexGame.savesData.BestScores,
                        overwrite: true);

                    UpdateSelectedAnimal(selectedID: YandexGame.savesData.SelectedAnimalID);
                }

                foreach (int window in _world.Where(out AnimalsShopWindowAspect windowAspect))
                {
                    Debug.Log("Load Purchases");

                    ref Purchases purchases = ref windowAspect.AnimalPurchases.Get(window);

                    IEnumerable<int> purchaseds = purchases.Entities.Where(static purchaseID =>
                    {
                        EcsDefaultWorld world = EcsDefaultWorldSingletonProvider.Instance.Get();

                        ref readonly Purchase purchase = ref world.GetPool<Purchase>().Read(purchaseID);

                        return YandexGame.savesData.PurchasedAnimals.Contains(purchase.Index);
                    });

                    foreach (int purchased in purchaseds)
                        _world.GetPool<PurchasedMarker>().Add(purchased);

                    ref SetupScrollRequest scrollSetupRequest = ref windowAspect.SetupScroll.Add(window);
                    scrollSetupRequest.ScrollToIndex = YandexGame.savesData.SelectedAnimalID;
                    scrollSetupRequest.Items = purchases.Entities;
                    scrollSetupRequest.IsAutoScroll = true;
                }

                foreach (int reward in _world.Where(out RewardAspect _))
                {
                    RewardUtils.Update(
                        rewardable: reward,
                        new UpdateRewardRequest
                        {
                            CollectionTime = YandexGame.savesData.RewardCollectionTime,
                            Count = YandexGame.savesData.RewardCount
                        });
                }

                foreach (int settings in _world.Where(out SettingsAspect settingsAspect))
                    UpdateAudioSettings(settingsAspect, settings);
            }
        }

        private void UpdateAudioSettings(SettingsAspect settingsAspect, int settings)
        {
            int request = _world.NewEntity();

            UpdateAudioSettingsRequestAspect requestAspect = _world.GetAspect<UpdateAudioSettingsRequestAspect>();

            requestAspect.UpdateAudioSettings.Add(request) = new UpdateAudioSettingsRequest
            {
                IsMusicOn = YandexGame.savesData.Audio.MusicIsOn,
                IsSoundOn = YandexGame.savesData.Audio.SoundIsOn
            };

            settingsAspect.GameAudioUpdatedEvent.Add(settings);
        }

        private void UpdateSelectedAnimal(ushort selectedID)
        {
            foreach (int player in _world.Where(out PlayerAspect playerAspect))
            {
                ref SelectedAnimal selectedAnimal = ref playerAspect.SelectedAnimals.Get(player);
                selectedAnimal.ID = selectedID;
                selectedAnimal.Prefab = playerAspect.AnimalPrefabs.Read(player).Animals[selectedAnimal.ID];

                YandexGame.savesData.SelectedAnimalID = selectedAnimal.ID;
                YandexGame.SaveProgress();
            }
        }
    }
}