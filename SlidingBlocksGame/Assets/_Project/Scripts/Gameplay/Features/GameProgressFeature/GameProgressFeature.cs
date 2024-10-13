using _Project.Scripts.Gameplay.Features.AudioFeature.Systems;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.Components;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.IntegrationFeatures.AudioFeature.Components;
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
                .AutoDelEntityComponent<ScoreUpdatedEvent>()
                .AddUnique(new ScoresSystem())
                .AutoDelEntityComponent<UpdateScoresRequest>()
                //
                .AddUnique(new ScoreRecordCheckSystem())
                .AutoDelEntityComponent<BestScoreUpdatedEvent>()
                .AddUnique(new BestScoreSystem())
                .AutoDelEntityComponent<UpdateBestScoreRequest>()
                //
                .AutoDelTag<GameAudioUpdatedEvent>()
                .AddUnique(new UpdateGameAudioStatusSystem())
                .AutoDelEntityComponent<UpdateGameAudioRequest>()
                // ui feature
                .AddUnique(new DisplayAudioButtonsStatusSystem())
                .AddUnique(new DisplayCoinsSystem())
                .AddUnique(new DisplayScoresSystem())
                // audio feature
                .AddAudioSystem<CoinCollectedEvent, CoinCollectedAudioConfig>();
        }
    }
}