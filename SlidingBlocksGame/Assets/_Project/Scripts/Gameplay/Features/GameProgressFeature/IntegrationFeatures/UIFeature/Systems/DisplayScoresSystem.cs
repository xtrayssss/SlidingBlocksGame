using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.Components;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameProgressFeature.IntegrationFeatures.UIFeature.Systems
{
    public class DisplayScoresSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class ScoreUpdatedEventAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<TargetEntity> Displayables;
            [Inc] public readonly EcsPool<ScoreUpdatedEvent> ScoresUpdatedEvent;
        }

        private class BestScoreUpdatedEventAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<TargetEntity> Displayables;
            [Inc] public readonly EcsPool<BestScoreUpdatedEvent> BestScoreUpdatedEvent;
        }

        private class ScoreWidgetAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<RectTransformRef> RectTransforms;
            [Inc] public readonly EcsPool<ScoreWidget> ScoreWidgets;
        }  
        
        private class BestScoreWidgetAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<RectTransformRef> RectTransforms;
            [Inc] public readonly EcsPool<BestScoreWidget> BestScoreWidgets;
        }

        private class DisplayableAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsPool<Scores> Scores;
            [Opt] public readonly EcsPool<BestScore> BestScores;
        }

        public void Run()
        {
            foreach (int @event in _world.Where(out ScoreUpdatedEventAspect scoreUpdatedEventAspect))
            {
                foreach (int widget in _world.Where(out ScoreWidgetAspect widgetAspect))
                {
                    if (!scoreUpdatedEventAspect.Displayables.Read(@event).Value.TryGetID(out int displayableID))
                        continue;

                    DisplayableAspect displayableAspect = _world.GetAspect<DisplayableAspect>();

                    ref ScoreWidget scoreWidget = ref widgetAspect.ScoreWidgets.Get(widget);

                    scoreWidget.AmountText.text = displayableAspect.Scores.Get(displayableID).Value.ToString();

                    Tween.PunchScale(
                        target: widgetAspect.RectTransforms.Read(widget).Value,
                        strength: new Vector3(0.5f, 0.5f),
                        duration: 0.2f);
                }
            }

            foreach (int @event in _world.Where(out BestScoreUpdatedEventAspect bestScoreUpdatedEventAspect))
            {
                foreach (int widget in _world.Where(out BestScoreWidgetAspect widgetAspect))
                {
                    if (!bestScoreUpdatedEventAspect.Displayables.Read(@event).Value.TryGetID(out int displayableID))
                        continue;

                    DisplayableAspect displayableAspect = _world.GetAspect<DisplayableAspect>();

                    ref BestScoreWidget bestScoreWidget = ref widgetAspect.BestScoreWidgets.Get(widget);

                    bestScoreWidget.AmountText.text = displayableAspect.BestScores.Get(displayableID).Value.ToString();

                    if (bestScoreUpdatedEventAspect.BestScoreUpdatedEvent.Read(@event).Delta != 0)
                    {
                        Tween.PunchScale(
                            target: widgetAspect.RectTransforms.Read(widget).Value,
                            strength: new Vector3(0.5f, 0.5f),
                            duration: 0.2f);
                    }
                }
            }
        }
    }
}