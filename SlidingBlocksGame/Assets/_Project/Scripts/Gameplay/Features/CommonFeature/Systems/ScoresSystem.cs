using _Project.Scripts.Gameplay.Features.CollectFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Systems
{
    public class ScoresSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class RequestAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<UpdateScoresRequest> UpdateScoresRequest;
            [Inc] public readonly EcsPool<TargetEntity> TargetEntities;
        }

        private class TargetEntityAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Scores> Scores;

            [Opt] public readonly EcsPool<TargetEntity> TargetEntity;
            [Opt] public readonly EcsPool<ScoresUpdatedEvent> ScoresUpdatedEvent;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out RequestAspect requestAspect))
            {
                ref readonly TargetEntity targetEntity = ref requestAspect.TargetEntities.Read(entity);

                if (!targetEntity.Value.TryGetID(out int targetID))
                    continue;

                ref readonly UpdateScoresRequest
                    updateScoresRequest = ref requestAspect.UpdateScoresRequest.Read(entity);

                TargetEntityAspect targetEntityAspect = _world.GetAspect<TargetEntityAspect>();

                ref Scores scores = ref targetEntityAspect.Scores.Get(targetID);

                int lastScores = scores.Value;

                if (updateScoresRequest.Overwrite)
                    scores.Value = updateScoresRequest.Value;
                else
                    scores.Value += updateScoresRequest.Value;

                int @event = _world.NewEntity();
                targetEntityAspect.ScoresUpdatedEvent.Add(@event).Delta = scores.Value - lastScores;
                targetEntityAspect.TargetEntity.Add(@event).Value = targetEntity.Value;
            }
        }
    }
}