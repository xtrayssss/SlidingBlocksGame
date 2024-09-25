using _Project.Scripts.Gameplay.Features.CollectFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class DisplayProgressSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class CoinsUpdatedEventAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<TargetEntity> TargetEntities;
            [Inc] public readonly EcsPool<CoinsUpdatedEvent> CoinsUpdatedEvent;
        }

        private class ScoreUpdatedEventAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<TargetEntity> TargetEntities;
            [Inc] public readonly EcsPool<ScoreUpdatedEvent> ScoresUpdatedEvent;
        }

        private class BestScoreUpdatedEventAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<TargetEntity> TargetEntities;
            [Inc] public readonly EcsPool<BestScoreUpdatedEvent> BestScoreUpdatedEvent;
        }

        private class ScoreUIAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ScoreUITag))]
            [Inc] public readonly EcsPool<TextMeshProUGUIRef> Texts;
            [Inc] public readonly EcsPool<RectTransformRef> RectTransforms;
        }

        private class BestScoreUIAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(BestScoreUITag))]
            [Inc] public readonly EcsPool<TextMeshProUGUIRef> Texts;

            [Inc] public readonly EcsPool<RectTransformRef> RectTransforms;
        }

        private class CoinUIAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CoinUITag))]
            [Inc] public readonly EcsPool<TextMeshProUGUIRef> Texts;

            [Inc] public readonly EcsPool<RectTransformRef> RectTransforms;
        }

        private class TargetEntityAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsPool<Coins> Coins;
            [Opt] public readonly EcsPool<Scores> Scores;
            [Opt] public readonly EcsPool<BestScore> BestScores;
        }

        public void Run()
        {
            foreach (int @event in _world.Where(out CoinsUpdatedEventAspect coinsUpdatedEventAspect))
            {
                foreach (int entity in _world.Where(out CoinUIAspect uiAspect))
                {
                    if (!coinsUpdatedEventAspect.TargetEntities.Read(@event).Value.TryGetID(out int targetID))
                        continue;

                    TargetEntityAspect targetEntityAspect = _world.GetAspect<TargetEntityAspect>();

                    uiAspect.Texts.Get(entity).Value.text =
                        targetEntityAspect.Coins.Read(targetID).Value.ToString();

                    if (coinsUpdatedEventAspect.CoinsUpdatedEvent.Read(@event).Delta != 0)
                    {
                        Tween.PunchScale(
                            target: uiAspect.RectTransforms.Read(entity).Value,
                            strength: new Vector3(0.5f, 0.5f),
                            duration: 0.2f);
                    }
                }
            }

            foreach (int @event in _world.Where(out ScoreUpdatedEventAspect scoreUpdatedEventAspect))
            {
                foreach (int entity in _world.Where(out ScoreUIAspect uiAspect))
                {
                    if (!scoreUpdatedEventAspect.TargetEntities.Read(@event).Value.TryGetID(out int targetID))
                        continue;

                    TargetEntityAspect targetEntityAspect = _world.GetAspect<TargetEntityAspect>();

                    uiAspect.Texts.Get(entity).Value.text = targetEntityAspect.Scores.Get(targetID).Value.ToString();
                    
                    Tween.PunchScale(
                        target: uiAspect.RectTransforms.Read(entity).Value,
                        strength: new Vector3(0.5f, 0.5f),
                        duration: 0.2f);
                }
            }

            foreach (int @event in _world.Where(out BestScoreUpdatedEventAspect bestScoreUpdatedEventAspect))
            {
                foreach (int entity in _world.Where(out BestScoreUIAspect uiAspect))
                {
                    if (!bestScoreUpdatedEventAspect.TargetEntities.Read(@event).Value.TryGetID(out int targetID))
                        continue;

                    TargetEntityAspect targetEntityAspect = _world.GetAspect<TargetEntityAspect>();

                    BestScoreUIAspect bestScoreUIAspect = _world.GetAspect<BestScoreUIAspect>();

                    bestScoreUIAspect.Texts.Get(entity).Value.text =
                        targetEntityAspect.BestScores.Get(targetID).Value.ToString();

                    if (bestScoreUpdatedEventAspect.BestScoreUpdatedEvent.Read(@event).Delta != 0)
                    {
                        Tween.PunchScale(
                            target: uiAspect.RectTransforms.Read(entity).Value,
                            strength: new Vector3(0.5f, 0.5f),
                            duration: 0.2f);
                    }
                }
            }
        }
    }
}