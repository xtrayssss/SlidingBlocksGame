using _Project.Scripts.Gameplay.Features.AnimalFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Systems;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Utils
{
    public static class ProgressUtils
    {
        private class ProgressAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsPool<UpdateCoinsRequest> UpdateCoinsRequest;

            [Opt] public readonly EcsPool<UpdateScoresRequest> UpdateScoresRequest;

            [Opt] public readonly EcsPool<UpdateBestScoreRequest> UpdateBestScoreRequest;

            [Opt] public readonly EcsPool<UpdateSelectedAnimalRequest> UpdateSelectedAnimalRequest;

            [Opt] public readonly EcsTagPool<ClearPurchasesRequest> ClearPurchasesRequest;

            [Opt] public readonly EcsPool<UpdateRewardRequest> UpdateRewardRequest;

            [Opt] public readonly EcsPool<UpdateGameAudioRequest> UpdateGameAudioRequest;

            [Opt] public readonly EcsPool<TargetEntity> TargetEntity;
        }

        public static int UpdateCoins(int target, int coins, bool overwrite = false)
        {
            EcsDefaultWorld world = EcsDefaultWorldSingletonProvider.Instance.Get();

            ProgressAspect progressAspect = world.GetAspect<ProgressAspect>();

            int @event = world.NewEntity();
            ref UpdateCoinsRequest updateCoinsRequest = ref progressAspect.UpdateCoinsRequest.Add(@event);
            updateCoinsRequest.Value = coins;
            updateCoinsRequest.Overwrite = overwrite;
            progressAspect.TargetEntity.Add(@event).Value = target.ToEntityLong(world);

            return @event;
        }

        public static void UpdateScores(int target, int scores, bool overwrite = false)
        {
            EcsDefaultWorld world = EcsDefaultWorldSingletonProvider.Instance.Get();

            ProgressAspect progressAspect = world.GetAspect<ProgressAspect>();

            int @event = world.NewEntity();
            ref UpdateScoresRequest updateScoresRequest = ref progressAspect.UpdateScoresRequest.Add(@event);
            updateScoresRequest.Value = scores;
            updateScoresRequest.Overwrite = overwrite;
            progressAspect.TargetEntity.Add(@event).Value = target.ToEntityLong(world);
        }

        public static void ClearPurchases(int target)
        {
            EcsDefaultWorld world = EcsDefaultWorldSingletonProvider.Instance.Get();

            ProgressAspect progressAspect = world.GetAspect<ProgressAspect>();

            int @event = world.NewEntity();
            progressAspect.ClearPurchasesRequest.Add(@event);
            progressAspect.TargetEntity.Add(@event).Value = target.ToEntityLong(world);
        }

        public static void UpdateSelectedAnimal(int target, ushort selectedID)
        {
            EcsDefaultWorld world = EcsDefaultWorldSingletonProvider.Instance.Get();

            ProgressAspect progressAspect = world.GetAspect<ProgressAspect>();

            int @event = world.NewEntity();

            ref UpdateSelectedAnimalRequest updateSelectedAnimalRequest =
                ref progressAspect.UpdateSelectedAnimalRequest.Add(@event);

            updateSelectedAnimalRequest.SelectedID = selectedID;
            progressAspect.TargetEntity.Add(@event).Value = target.ToEntityLong(world);
        }


        public static void UpdateReward(int target, long time, int count, bool overwrite = false)
        {
            EcsDefaultWorld world = EcsDefaultWorldSingletonProvider.Instance.Get();

            ProgressAspect progressAspect = world.GetAspect<ProgressAspect>();

            int @event = world.NewEntity();

            ref UpdateRewardRequest updateRewardRequest = ref progressAspect.UpdateRewardRequest.Add(@event);

            updateRewardRequest.Count = count;
            updateRewardRequest.Time = time;
            updateRewardRequest.Overwrite = overwrite;
            progressAspect.TargetEntity.Add(@event).Value = target.ToEntityLong(world);
        }

        public static void UpdateGameAudio(int target, bool isMusicOn, bool isSoundOn)
        {
            EcsDefaultWorld world = EcsDefaultWorldSingletonProvider.Instance.Get();

            ProgressAspect progressAspect = world.GetAspect<ProgressAspect>();

            int request = world.NewEntity();

            ref UpdateGameAudioRequest updateGameAudioRequest = ref progressAspect.UpdateGameAudioRequest.Add(request);

            updateGameAudioRequest.IsMusicOn = isMusicOn;
            updateGameAudioRequest.IsSoundOn = isSoundOn;
            
            progressAspect.TargetEntity.Add(request).Value = target.ToEntityLong(world);
        }

        public static void UpdateBestScore(int target, int scores, bool overwrite = false)
        {
            EcsDefaultWorld world = EcsDefaultWorldSingletonProvider.Instance.Get();

            ProgressAspect progressAspect = world.GetAspect<ProgressAspect>();

            int @event = world.NewEntity();
            ref UpdateBestScoreRequest updateScoresRequest = ref progressAspect.UpdateBestScoreRequest.Add(@event);
            updateScoresRequest.Value = scores;
            updateScoresRequest.Overwrite = overwrite;
            progressAspect.TargetEntity.Add(@event).Value = target.ToEntityLong(world);
        }
    }
}