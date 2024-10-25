using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.Components;
using _Project.Scripts.Gameplay.Features.ScoreFeature.Components;
using _Project.Scripts.Gameplay.Features.ScoreFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.ScoreFeature.IntegrationFeatures.UIFeature.Systems
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
            [Inc] public readonly EcsPool<BestScoreUpdatedEvent> BestScoreUpdatedEvent;
        }

        private class ScoreWidgetAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<ScoreWidget> ScoreWidgets;
            [Inc] public readonly EcsPool<UIElement> UIElements;
        }

        private class BestScoreWidgetAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<BestScoreWidget> BestScoreWidgets;
            [Inc] public readonly EcsPool<UIElement> UIElements;
        }

        private class DisplayableAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsPool<Scores> Scores;
        }
        
        private class PlayerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayerTag))]
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

                    ref UIElement uiElement = ref widgetAspect.UIElements.Get(widget);

                    Tween.PunchScale(
                        target: uiElement.RectTransform,
                        strength: new Vector3(0.5f, 0.5f),
                        duration: 0.2f);
                }
            }

            foreach (int _ in _world.Where(out BestScoreUpdatedEventAspect _))
            {
                foreach (int widget in _world.Where(out BestScoreWidgetAspect widgetAspect))
                {
                    foreach (int player in _world.Where(out PlayerAspect playerAspect))
                    {
                        ref BestScoreWidget bestScoreWidget = ref widgetAspect.BestScoreWidgets.Get(widget);

                        bestScoreWidget.AmountText.text = playerAspect.BestScores.Get(player).Value.ToString();

                        ref UIElement uiElement = ref widgetAspect.UIElements.Get(widget);

                        Tween.PunchScale(
                            target: uiElement.RectTransform,
                            strength: new Vector3(0.5f, 0.5f),
                            duration: 0.2f);
                    }
                }
            }
        }
    }
}