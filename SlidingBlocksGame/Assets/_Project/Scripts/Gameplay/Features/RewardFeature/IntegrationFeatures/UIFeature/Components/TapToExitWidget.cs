using System;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.UIFeature.Components
{
    [Serializable]
    [MetaGroup("UI")]
    public struct TapToExitWidget : IEcsComponent
    {
        public Button ExitButton;
        public Tween WobbleTween;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components",
            sourceClassName: "TapToExitWidget/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<TapToExitWidget>
        {
        }
    }
}