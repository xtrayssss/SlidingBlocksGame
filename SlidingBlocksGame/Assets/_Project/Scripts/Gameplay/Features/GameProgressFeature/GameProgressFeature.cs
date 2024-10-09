using _Project.Scripts.Gameplay.Features.AudioBaseFeature.Systems;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.Components;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.IntegrationFeatures.AudioFeature.Components;
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
                // audio feature
                .AddAudioSystem<CoinCollectedEvent, CollectedAudioConfig>();
        }
    }
}