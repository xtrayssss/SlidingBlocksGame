using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;

namespace _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.UIFeature.Components
{
    [Serializable]
    [MetaGroup("UI")]
    public struct TapToExitWidget : IEcsComponent
    {
        public Button ExitButton;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components",
            sourceClassName: "TapToExitWidget/Template", sourceAssembly: "Assembly-CSharp")]

        private sealed class Template : ComponentTemplate<TapToExitWidget>
        {
        }
    }
}