using _Project.Scripts.Gameplay.Features.CollectionFeature.Systems;
using _Project.Scripts.Gameplay.Features.ScoreFeature.Components;
using _Project.Scripts.Gameplay.Features.ScoreFeature.Systems;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.ScoreFeature
{
    public class ScoreFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AutoDelEntityComponent<ScoreUpdatedEvent>()
                .AddUnique(new ScoresSystem())
                .AutoDelEntityComponent<UpdateScoresRequest>()
                //
                .AddUnique(new BestScoreCheckSystem())
                .AutoDelEntityComponent<BestScoreUpdatedEvent>()
                .AddUnique(new BestScoreSystem())
                .AutoDelEntityComponent<UpdateBestScoreRequest>();
        }
    }
}