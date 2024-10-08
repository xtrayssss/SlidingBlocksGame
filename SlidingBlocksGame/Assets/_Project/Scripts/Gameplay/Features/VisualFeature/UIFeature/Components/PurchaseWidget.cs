using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components
{
    [Serializable]
    [MetaGroup("UI")]
    public struct PurchaseWidget : IEcsComponent
    {
        public PurchaseStatusWidget PurchaseStatusWidget;
        public RawImage Icon;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components",
            sourceClassName: "PurchaseWidget/Template", sourceAssembly: "Assembly-CSharp")]

        private sealed class Template : ComponentTemplate<PurchaseWidget>
        {
        }
    }
}