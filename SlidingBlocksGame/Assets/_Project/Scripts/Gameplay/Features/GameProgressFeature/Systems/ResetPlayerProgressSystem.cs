using _Project.Scripts.Gameplay.Features.AnimalFeature.Components;
using _Project.Scripts.Gameplay.Features.CoinFeature.Components;
using _Project.Scripts.Gameplay.Features.CoinFeature.Utils;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.Utils;
using _Project.Scripts.Gameplay.Features.ScoreFeature.Components;
using _Project.Scripts.Gameplay.Features.ScoreFeature.Utils;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.ButtonFeature.Components;
using DCFApixels.DragonECS;
using YG;

namespace _Project.Scripts.Gameplay.Features.GameProgressFeature.Systems
{
    public class ResetPlayerProgressSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class ResetButtonAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> ButtonClicked;
            [Inc] public readonly EcsTagPool<ResetProgressButtonTag> ResetProgressButtonTag;
        }

        private class PlayerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayerTag))]
            [Inc] public readonly EcsPool<Coins> Coins;
            [Inc] public readonly EcsPool<BestScore> BestScores;
            [Inc] public readonly EcsPool<SelectedAnimal> SelectedAnimals;
            [Inc] public readonly EcsPool<AnimalPrefabs> AnimalPrefabs;
        }

        private class RewardAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Reward> Rewards;
        }

        private class PurchasedAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PurchaseTag))]
            [Inc] public readonly EcsTagPool<PurchasedMarker> PurchasedMarker;
        }

        public void Run()
        {
            if (!IsResetButtonClicked())
                return;

            ResetAllProgress();
        }

        private bool IsResetButtonClicked()
        {
            foreach (int _ in _world.Where(out ResetButtonAspect _))
                return true;
            
            return false;
        }

        private void ResetAllProgress()
        {
            ResetPlayerProgress();
            ResetRewards();
            SaveProgress();
        }

        private void ResetPlayerProgress()
        {
            foreach (int player in _world.Where(out PlayerAspect playerAspect))
            {
                ResetPlayerCoins(player, playerAspect);
                ResetPlayerScore(player, playerAspect);
                ResetPlayerPurchases();
                ResetSelectedAnimal(player, playerAspect);
            }
        }

        private static void ResetPlayerCoins(int player, PlayerAspect playerAspect)
        {
            if (playerAspect.Coins.Get(player).Value != 0)
            {
                CoinUtils.Update(
                    coinable: player,
                    coins: 0,
                    overwrite: true);
            }
        }

        private static void ResetPlayerScore(int player, PlayerAspect playerAspect)
        {
            if (playerAspect.BestScores.Get(player).Value != 0)
            {
                ScoreUtils.UpdateBestScore(
                    scorable: player,
                    score: 0,
                    overwrite: true);
            }
        }

        private void ResetPlayerPurchases()
        {
            foreach (int purchase in _world.Where(out PurchasedAspect purchasedAspect)) 
                purchasedAspect.PurchasedMarker.Del(purchase);

            YandexGame.savesData.PurchasedAnimals.Clear();
        }

        private static void ResetSelectedAnimal(int player, PlayerAspect playerAspect)
        {
            const ushort DEFAULT_ANIMAL_ID = 0;
            
            ref SelectedAnimal selectedAnimal = ref playerAspect.SelectedAnimals.Get(player);
            selectedAnimal.ID = DEFAULT_ANIMAL_ID;
            selectedAnimal.Prefab = playerAspect.AnimalPrefabs.Read(player).Animals[DEFAULT_ANIMAL_ID];

            YandexGame.savesData.SelectedAnimalID = DEFAULT_ANIMAL_ID;
        }

        private void ResetRewards()
        {
            foreach (int reward in _world.Where(out RewardAspect _))
            {
                RewardUtils.Update(
                    rewardable: reward,
                    new UpdateRewardRequest
                    {
                        CollectionTime = 0,
                        Count = 0,
                        Overwrite = true
                    });
            }
        }

        private static void SaveProgress() => 
            YandexGame.SaveProgress();
    }
}