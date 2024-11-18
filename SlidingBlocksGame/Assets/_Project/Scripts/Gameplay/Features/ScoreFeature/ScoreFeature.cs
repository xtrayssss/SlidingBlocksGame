using _Project.Scripts.Gameplay.Features.ScoreFeature.Components;
using _Project.Scripts.Gameplay.Features.ScoreFeature.IntegrationFeatures.UIFeature.Systems;
using _Project.Scripts.Gameplay.Features.ScoreFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.ScoreFeature
{
    public class ScoreFeature<TMask> : EcsModule<TMask> where TMask : EcsAspect, new()
    {
        protected override void Import(Builder builder)
        {
            builder
                .AutoDelEntityComponent<ScoreUpdatedEvent>()
                .AddSystem(new ScoresSystem())
                .AutoDelEntityComponent<UpdateScoreRequest>()
                //
                .AddSystem(new ScoreRecordCheckSystem())
                .AutoDelEntityComponent<BestScoreUpdatedEvent>()
                .AddSystem(new BestScoreSystem())
                .AutoDelEntityComponent<UpdateBestScoreRequest>()
                
                // ui feature
                .AddSystem(new DisplayScoresSystem());
        }
    }
}