using System;
using DCFApixels.DragonECS;
using TMPro;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components
{
    [Serializable]
    [MetaGroup("UI")]
    public struct ScoreWidget : IEcsComponent
    {
        public TextMeshProUGUI AmountText;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components",
            sourceClassName: "ScoreWidget/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<ScoreWidget>
        {
        }
    }
}