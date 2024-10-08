using System;
using DCFApixels.DragonECS;
using TMPro;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components
{
    [Serializable]
    [MetaGroup("UI")]
    public struct RewardCoinsWidget : IEcsComponent
    {
        public TextMeshProUGUI AmountText;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components",
            sourceClassName: "RewardCoinsWidget/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<RewardCoinsWidget>
        {
        }
    }
}