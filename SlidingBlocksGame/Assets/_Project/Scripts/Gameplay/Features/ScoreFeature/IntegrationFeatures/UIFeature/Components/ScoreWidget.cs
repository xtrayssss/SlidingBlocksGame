using System;
using DCFApixels.DragonECS;
using TMPro;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.ScoreFeature.IntegrationFeatures.UIFeature.Components
{
    [Serializable]
    [MetaGroup("Score/UI")]
    public struct ScoreWidget : IEcsComponent
    {
        public TextMeshProUGUI AmountText;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.GameProgressFeature.IntegrationFeatures.UIFeature.Components",
            sourceClassName: "ScoreWidget/Template", sourceAssembly: "GameProgressFeature.UIFeature.Components")]
        private sealed class Template : ComponentTemplate<ScoreWidget>
        {
        }
    }
}