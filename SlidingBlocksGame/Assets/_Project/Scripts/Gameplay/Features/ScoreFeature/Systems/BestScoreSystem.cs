using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.Components;
using _Project.Scripts.Gameplay.Features.ScoreFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine.PlayerLoop;

namespace _Project.Scripts.Gameplay.Features.ScoreFeature.Systems
{
    public class BestScoreSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class RequestAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<UpdateBestScoreRequest> UpdateBestScoresRequest;
            [Inc] public readonly EcsPool<TargetEntity> TargetEntities;
        }

        private class PlayerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayerTag))]
            [Inc] public readonly EcsPool<BestScore> BestScores;
        }
        
        private class EventAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsPool<BestScoreUpdatedEvent> BestScoreUpdatedEvent;
        }

        public void Run()
        {
            foreach (int request in _world.Where(out RequestAspect requestAspect))
            {
                ref readonly UpdateBestScoreRequest updateRequest =
                    ref requestAspect.UpdateBestScoresRequest.Read(request);

                foreach (int player in _world.Where(out PlayerAspect playerAspect))
                {
                    ref BestScore bestScore = ref playerAspect.BestScores.Get(player);
                    
                    if (updateRequest.Overwrite)
                        bestScore.Value = updateRequest.Value;
                    else
                        bestScore.Value += updateRequest.Value;

                    GenerateEvent();
                }
            }
        }

        private void GenerateEvent()
        {
            EventAspect eventAspect = _world.GetAspect<EventAspect>();
            int @event = _world.NewEntity();
            eventAspect.BestScoreUpdatedEvent.Add(@event);
        }
    }
}