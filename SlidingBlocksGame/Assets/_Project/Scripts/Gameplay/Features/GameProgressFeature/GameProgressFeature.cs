using _Project.Scripts.Gameplay.Features.AudioFeature.Systems;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.Components;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.IntegrationFeatures.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.IntegrationFeatures.DestructionFeature.Systems;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.IntegrationFeatures.UIFeature.Systems;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameProgressFeature
{
    public class GameProgressFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                // score
                .AutoDelEntityComponent<ScoreUpdatedEvent>()
                .AddUnique(new ScoresSystem())
                .AutoDelEntityComponent<UpdateScoresRequest>()
                //
                .AddUnique(new ScoreRecordCheckSystem())
                .AutoDelEntityComponent<BestScoreUpdatedEvent>()
                .AddUnique(new BestScoreSystem())
                .AutoDelEntityComponent<UpdateBestScoreRequest>()

                // coins
                .AutoDelTag<CoinSpawnedEvent>()
                .AutoDelTag<CoinCollectAnimationCompletedEvent>()
                .AutoDelTag<CoinDestroyAnimationCompletedEvent>()
                .AddUnique(new CatchCoinSystem())
                .AddUnique(new CreateCoinSystem())
                .AutoDelTag<CreateCoinRequest>()
                // 
                .AutoDelTag<CoinCollectedEvent>()
                .AddUnique(new CollectCoinSystem())
                //
                .AutoDelEntityComponent<CoinsUpdatedEvent>()
                .AddUnique(new CoinsSystem())
                .AutoDelEntityComponent<UpdateCoinsRequest>()
                // coin destruction
                .AddUnique(new CoinDestroySystem())
                .AddUnique(new CoinDestructionSystem())
                // coin audio
                .AddAudioSystem<CoinCollectedEvent, CoinCollectedAudioConfig>()

                // ui feature
                .AddUnique(new DisplayAudioButtonsStatusSystem())
                .AddUnique(new DisplayCoinsSystem())
                .AddUnique(new DisplayScoresSystem())

                // audio 
                .AutoDelTag<GameAudioUpdatedEvent>()
                .AddUnique(new UpdateGameAudioStatusSystem())
                .AutoDelEntityComponent<UpdateGameAudioRequest>();
        }
    }
}