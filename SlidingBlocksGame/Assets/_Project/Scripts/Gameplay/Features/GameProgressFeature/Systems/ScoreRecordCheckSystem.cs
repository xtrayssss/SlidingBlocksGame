using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameProgressFeature.Systems
{
    public class ScoreRecordCheckSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class ScoreUpdateEventAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ScoreUpdatedEvent))]
            [Inc] public readonly EcsPool<TargetEntity> TargetEntities;
        }

        private class TargetEntityAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Scores> Scores;
            [Inc] public readonly EcsPool<BestScore> BestScores;
        }

        public void Run()
        {
            foreach (int @event in _world.Where(out ScoreUpdateEventAspect scoreUpdatedEventAspect))
            {
                if (!scoreUpdatedEventAspect.TargetEntities.Read(@event).Value.TryGetID(out int targetID))
                    continue;

                TargetEntityAspect targetEntityAspect = _world.GetAspect<TargetEntityAspect>();

                ref readonly Scores score = ref targetEntityAspect.Scores.Read(targetID);
                ref readonly BestScore bestScore = ref targetEntityAspect.BestScores.Read(targetID);

                if (score.Value > bestScore.Value)
                    ProgressUtils.UpdateBestScore(
                        targetID,
                        score.Value,
                        overwrite: true);
            }
        }
    }
}