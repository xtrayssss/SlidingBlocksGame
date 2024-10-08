using _Project.Scripts.Gameplay.Features.GameProgressFeature.Components;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.Systems;
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
                .AutoDelEntityComponent<UpdateBestScoreRequest>();
        }
    }
}