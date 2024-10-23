using System;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;

namespace _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.UIFeature.Components
{
    [Serializable]
    [MetaGroup("Reward/UI")]
    public struct RewardWindow : IEcsComponent
    {
        public EcsEntityConnect RewardCoinsWidgetConnect;
        public EcsEntityConnect RewardConfettiEffectConnect;
        public EcsEntityConnect CongratulationWidgetConnect;
        [FormerlySerializedAs("SunshineWidgetConnect")] public EcsEntityConnect SunshineConnect;
        public EcsEntityConnect TapToExitWidgetConnect;
        public Sequence OpenCloseTween;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components",
            sourceClassName: "RewardWindow/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<RewardWindow>
        {
        }
    }
}