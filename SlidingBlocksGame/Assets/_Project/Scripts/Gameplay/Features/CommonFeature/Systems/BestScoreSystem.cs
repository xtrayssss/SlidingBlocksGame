using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Systems
{
    public class BestScoreSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class RequestAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<UpdateBestScoreRequest> UpdateBestScoresRequest;
            [Inc] public readonly EcsPool<TargetEntity> TargetEntities;
        }

        private class TargetEntityAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<BestScore> BestScores;

            [Opt] public readonly EcsPool<TargetEntity> TargetEntity;
            [Opt] public readonly EcsPool<BestScoreUpdatedEvent> BestScoreUpdatedEvent;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out RequestAspect requestAspect))
            {
                ref readonly TargetEntity targetEntity = ref requestAspect.TargetEntities.Read(entity);

                if (!targetEntity.Value.TryGetID(out int targetID))
                    continue;

                ref readonly UpdateBestScoreRequest
                    updateScoresRequest = ref requestAspect.UpdateBestScoresRequest.Read(entity);

                TargetEntityAspect targetEntityAspect = _world.GetAspect<TargetEntityAspect>();

                ref BestScore bestScore = ref targetEntityAspect.BestScores.Get(targetID);

                int lastBestScore = bestScore.Value;

                if (updateScoresRequest.Overwrite)
                    bestScore.Value = updateScoresRequest.Value;
                else
                    bestScore.Value += updateScoresRequest.Value;

                int @event = _world.NewEntity();
                targetEntityAspect.TargetEntity.Add(@event).Value = targetEntity.Value;
                targetEntityAspect.BestScoreUpdatedEvent.Add(@event).Delta = bestScore.Value - lastBestScore;
            }
        }
    }
}