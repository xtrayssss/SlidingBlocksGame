using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.Components;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameProgressFeature.IntegrationFeatures.UIFeature.Systems
{
    public class DisplayCoinsSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class CoinsUpdatedEventAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<TargetEntity> Displayables;
            [Inc] public readonly EcsPool<CoinsUpdatedEvent> CoinsUpdatedEvent;
        }

        private class CoinWidgetAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<RectTransformRef> RectTransforms;
            [Inc] public readonly EcsPool<CoinWidget> CoinWidgets;
        }

        private class DisplayableAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsPool<Coins> Coins;
        }

        public void Run()
        {
            foreach (int @event in _world.Where(out CoinsUpdatedEventAspect coinsUpdatedEventAspect))
            {
                foreach (int widget in _world.Where(out CoinWidgetAspect widgetAspect))
                {
                    if (!coinsUpdatedEventAspect.Displayables.Read(@event).Value.TryGetID(out int displayableID))
                        continue;

                    DisplayableAspect displayableAspect = _world.GetAspect<DisplayableAspect>();

                    ref CoinWidget coinWidget = ref widgetAspect.CoinWidgets.Get(widget);
                    coinWidget.AmountText.text = displayableAspect.Coins.Read(displayableID).Value.ToString();

                    if (coinsUpdatedEventAspect.CoinsUpdatedEvent.Read(@event).Delta != 0)
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