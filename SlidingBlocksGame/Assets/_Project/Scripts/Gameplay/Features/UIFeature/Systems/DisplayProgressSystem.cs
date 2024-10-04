using _Project.Scripts.Gameplay.Features.CollectionFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.ScoreFeature.Components;
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

        private class ScoreWidgetAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<RectTransformRef> RectTransforms;
            [Inc] public readonly EcsPool<ScoreWidget> ScoreWidgets;
        }
        
        private class CoinWidgetAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<RectTransformRef> RectTransforms;
            [Inc] public readonly EcsPool<CoinWidget> CoinWidgets;
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
                foreach (int widget in _world.Where(out CoinWidgetAspect widgetAspect))
                {
                    if (!coinsUpdatedEventAspect.TargetEntities.Read(@event).Value.TryGetID(out int targetID))
                        continue;

                    TargetEntityAspect targetEntityAspect = _world.GetAspect<TargetEntityAspect>();

                    ref CoinWidget coinWidget = ref widgetAspect.CoinWidgets.Get(widget);
                    coinWidget.AmountText.text = targetEntityAspect.Coins.Read(targetID).Value.ToString();

                    if (coinsUpdatedEventAspect.CoinsUpdatedEvent.Read(@event).Delta != 0)
                    {
                        Tween.PunchScale(
                            target: widgetAspect.RectTransforms.Read(widget).Value,
                            strength: new Vector3(0.5f, 0.5f),
                            duration: 0.2f);
                    }
                }
            }

            foreach (int @event in _world.Where(out ScoreUpdatedEventAspect scoreUpdatedEventAspect))
            {
                foreach (int widget in _world.Where(out ScoreWidgetAspect widgetAspect))
                {
                    if (!scoreUpdatedEventAspect.TargetEntities.Read(@event).Value.TryGetID(out int targetID))
                        continue;

                    TargetEntityAspect targetEntityAspect = _world.GetAspect<TargetEntityAspect>();

                    ref ScoreWidget scoreWidget = ref widgetAspect.ScoreWidgets.Get(widget);

                    scoreWidget.AmountText.text = targetEntityAspect.Scores.Get(targetID).Value.ToString();
                    
                    //scoreWidget.Value.text = targetEntityAspect.BestScores.Get(targetID).Value.ToString();

                    Tween.PunchScale(
                        target: widgetAspect.RectTransforms.Read(widget).Value,
                        strength: new Vector3(0.5f, 0.5f),
                        duration: 0.2f);
                }
            }
        }
    }
}