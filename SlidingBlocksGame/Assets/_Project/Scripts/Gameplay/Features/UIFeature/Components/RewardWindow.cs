using System;
using DCFApixels.DragonECS;
using PrimeTween;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    [MetaGroup("UI")]
    public struct RewardWindow : IEcsComponent
    {
        public EcsEntityConnect RewardCoinsWidgetConnect;
        public EcsEntityConnect RewardConfettiEffectConnect;
        public EcsEntityConnect CongratulationWidgetConnect;
        public EcsEntityConnect SunshineWidgetConnect;
        public EcsEntityConnect TapToExitWidgetConnect;
        public Sequence OpenCloseTween;

        private sealed class Template : ComponentTemplate<RewardWindow>
        {
        }
    }
}