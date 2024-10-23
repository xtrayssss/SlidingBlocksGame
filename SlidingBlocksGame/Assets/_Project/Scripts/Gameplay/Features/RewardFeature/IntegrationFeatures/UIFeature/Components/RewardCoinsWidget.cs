using System;
using DCFApixels.DragonECS;
using PrimeTween;
using TMPro;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.UIFeature.Components
{
    [Serializable]
    [MetaGroup("Reward/UI")]
    public struct RewardCoinsWidget : IEcsComponent
    {
        public TextMeshProUGUI AmountText;
        public Tween WobbleTween;
        
        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components",
            sourceClassName: "RewardCoinsWidget/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<RewardCoinsWidget>
        {
        }
    }
}