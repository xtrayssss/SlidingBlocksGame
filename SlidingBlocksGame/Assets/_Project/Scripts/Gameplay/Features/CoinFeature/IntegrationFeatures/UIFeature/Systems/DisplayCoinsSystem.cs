using _Project.Scripts.Gameplay.Features.CoinFeature.Components;
using _Project.Scripts.Gameplay.Features.CoinFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.CoinFeature.IntegrationFeatures.UIFeature.Systems
{
    public class DisplayCoinsSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class CoinsUpdatedEventAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<TargetEntity> Coinables;
            [Inc] public readonly EcsTagPool<CoinsUpdatedEvent> CoinsUpdatedEvent;
        }

        private class CoinWidgetAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<CoinWidget> CoinWidgets;
            [Inc] public readonly EcsPool<UIElement> UIElements;
        }

        private class CoinableAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsPool<Coins> Coins;
        }

        public void Run()
        {
            foreach (int @event in _world.Where(out CoinsUpdatedEventAspect eventAspect))
            {
                foreach (int widget in _world.Where(out CoinWidgetAspect widgetAspect))
                {
                    if (!eventAspect.Coinables.Read(@event).Value.TryGetID(out int coinableID))
                        continue;

                    CoinableAspect coinableAspect = _world.GetAspect<CoinableAspect>();

                    ref CoinWidget coinWidget = ref widgetAspect.CoinWidgets.Get(widget);
                    coinWidget.AmountText.text = coinableAspect.Coins.Read(coinableID).Value.ToString();

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