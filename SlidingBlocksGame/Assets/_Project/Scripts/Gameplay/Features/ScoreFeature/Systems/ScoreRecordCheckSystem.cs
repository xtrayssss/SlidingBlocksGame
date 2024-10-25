using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.ScoreFeature.Components;
using _Project.Scripts.Gameplay.Features.ScoreFeature.Utils;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.ScoreFeature.Systems
{
    public class ScoreRecordCheckSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class ScoreUpdatedAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ScoreUpdatedEvent))]
            [Inc] public readonly EcsPool<TargetEntity> Scorables;
        }

        private class ScorableAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Scores> Scores;
            [Inc] public readonly EcsPool<BestScore> BestScores;
        }

        public void Run()
        {
            foreach (int @event in _world.Where(out ScoreUpdatedAspect scoreUpdatedEventAspect))
            {
                if (!scoreUpdatedEventAspect.Scorables.Read(@event).Value.TryGetID(out int scorableID))
                    continue;

                ScorableAspect scorableAspect = _world.GetAspect<ScorableAspect>();

                ref readonly Scores score = ref scorableAspect.Scores.Read(scorableID);
                ref readonly BestScore bestScore = ref scorableAspect.BestScores.Read(scorableID);

                if (score.Value > bestScore.Value)
                {
                    ScoreUtils.UpdateBestScore(
                        scorable: scorableID,
                        score: score.Value,
                        overwrite: true);
                }
            }
        }
    }
}