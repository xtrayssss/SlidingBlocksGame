using System;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.UIFeature.Components
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

        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components",
            sourceClassName: "RewardWindow/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<RewardWindow>
        {
        }
    }
}