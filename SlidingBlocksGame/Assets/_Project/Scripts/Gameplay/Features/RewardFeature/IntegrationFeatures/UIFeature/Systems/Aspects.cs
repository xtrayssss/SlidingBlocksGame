using _Project.Scripts.Gameplay.Features.CoinFeature.Components;
using _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components;
using DCFApixels.DragonECS;

// ReSharper disable UnassignedReadonlyField

namespace _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.UIFeature.Systems
{
    public class RewardWindowAspect : EcsAspectAuto
    {
        [Inc] public readonly EcsPool<GameObjectConnect> GoConnects;
        [Inc] public readonly EcsPool<RewardWindow> RewardWindows;
    }

    public class RewardCoinsAspect : EcsAspectAuto
    {
        [Inc] public readonly EcsPool<Coins> CoinsDisplay;
        [Inc] public readonly EcsPool<RewardCoinsWidget> RewardCoinsWidgets;
        [Inc] public readonly EcsPool<UIElement> UIElements;
        [Inc] public readonly EcsPool<OriginalAnchoredPosition> OriginalAnchoredPositions;
    }

    public class TapToExitAspect : EcsAspectAuto
    {
        [Inc] public readonly EcsPool<TapToExitWidget> TapToExitWidgets;
        [Inc] public readonly EcsPool<OriginalAnchoredPosition> OriginalAnchoredPositions;
        [Inc] public readonly EcsPool<UIElement> UIElements;
    }

    public class SunshineAspect : EcsAspectAuto
    {
        [Inc] public readonly EcsPool<Sunshine> Sunshine;
    }
}