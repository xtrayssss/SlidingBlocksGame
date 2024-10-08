using System;
using DCFApixels.DragonECS;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;


namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components
{
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

    [Serializable]
    [MetaGroup("UI")]
    public struct AnimalsShopWindow : IEcsComponent
    {
        public Sequence OpenCloseTween;
        public PurchaseStatusWidget PurchaseStatusWidget;
        
        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components",
            sourceClassName: "AnimalsShopWindow/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<AnimalsShopWindow>
        {
        }
    }
}