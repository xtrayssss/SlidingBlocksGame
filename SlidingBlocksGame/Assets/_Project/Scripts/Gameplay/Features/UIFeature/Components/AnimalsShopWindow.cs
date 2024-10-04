using System;
using DCFApixels.DragonECS;
using PrimeTween;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
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

        private sealed class Template : ComponentTemplate<AnimalsShopWindow>
        {
        }
    }
}