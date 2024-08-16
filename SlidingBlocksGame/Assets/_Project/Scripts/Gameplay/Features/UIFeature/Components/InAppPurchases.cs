using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct InAppPurchases : IEcsComponent
    {
        public Purchase[] Value;
        [Serializable]
        public struct Purchase
        {
            public float Value;
            public GameObject Prefab;
        }

        private sealed class Template : ComponentTemplate<InAppPurchases>
        {
        }
    }
}