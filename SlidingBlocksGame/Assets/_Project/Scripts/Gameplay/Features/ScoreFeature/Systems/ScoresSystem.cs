using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.ScoreFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.ScoreFeature.Systems
{
    public class ScoresSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class RequestAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<UpdateScoreRequest> UpdateScoresRequest;
            [Inc] public readonly EcsPool<TargetEntity> TargetEntities;
        }

        private class TargetEntityAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Scores> Scores;

            [Opt] public readonly EcsPool<TargetEntity> TargetEntity;
            [Opt] public readonly EcsPool<ScoreUpdatedEvent> ScoresUpdatedEvent;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out RequestAspect requestAspect))
            {
                ref readonly TargetEntity targetEntity = ref requestAspect.TargetEntities.Read(entity);

                if (!targetEntity.Value.TryGetID(out int targetID))
                    continue;

                ref readonly UpdateScoreRequest
                    updateScoreRequest = ref requestAspect.UpdateScoresRequest.Read(entity);

                TargetEntityAspect targetEntityAspect = _world.GetAspect<TargetEntityAspect>();

                ref Scores score = ref targetEntityAspect.Scores.Get(targetID);

                if (updateScoreRequest.Overwrite)
                    score.Value = updateScoreRequest.Value;
                else
                    score.Value += updateScoreRequest.Value;

                int @event = _world.NewEntity();
                targetEntityAspect.TargetEntity.Add(@event).Value = targetEntity.Value;
                targetEntityAspect.ScoresUpdatedEvent.Add(@event);
            }
        }
    }
}