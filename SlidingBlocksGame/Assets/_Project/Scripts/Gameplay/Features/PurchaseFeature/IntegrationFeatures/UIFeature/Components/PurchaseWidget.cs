using System;
using DCFApixels.DragonECS;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;

namespace _Project.Scripts.Gameplay.Features.PurchaseFeature.IntegrationFeatures.UIFeature.Components
{
    [Serializable]
    [MetaGroup("Purchase/UI")]
    public struct PurchaseWidget : IEcsComponent
    {
        public PurchaseStatusWidget PurchaseStatusWidget;
        public RawImage Icon;
        public Tween RotationTween;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components",
            sourceClassName: "PurchaseWidget/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<PurchaseWidget>
        {
        }
    }

    [Serializable]
    public struct PurchaseStatusWidget
    {
        public TextMeshProUGUI PriceText;

        public GameObject Lock;
        public GameObject Unlock;
        public GameObject Play;
        public GameObject Price;

        public GameObject Current;
    }
}