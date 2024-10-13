using _Project.Scripts.Gameplay.Features.CollectFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
<<<<<<<< HEAD:SlidingBlocksGame/Assets/_Project/Scripts/Gameplay/Features/CommonFeature/Systems/BestScoreCheckSystem.cs
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using _Project.Scripts.Gameplay.Utils;
========
using _Project.Scripts.Gameplay.Features.GameProgressFeature.Components;
>>>>>>>> recovery-branch:SlidingBlocksGame/Assets/_Project/Scripts/Gameplay/Features/GameProgressFeature/Systems/ScoreRecordCheckSystem.cs
using DCFApixels.DragonECS;
using Unity.Mathematics;

<<<<<<<< HEAD:SlidingBlocksGame/Assets/_Project/Scripts/Gameplay/Features/CommonFeature/Systems/BestScoreCheckSystem.cs
namespace _Project.Scripts.Gameplay.Features.CommonFeature.Systems
========
namespace _Project.Scripts.Gameplay.Features.GameProgressFeature.Systems
>>>>>>>> recovery-branch:SlidingBlocksGame/Assets/_Project/Scripts/Gameplay/Features/GameProgressFeature/Systems/ScoreRecordCheckSystem.cs
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