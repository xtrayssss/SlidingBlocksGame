using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct PurchaseButtonStatus : IEcsComponent
    {
        public GameObject Lock;
        public GameObject Unlock;
        public GameObject Play;

        private sealed class Template : ComponentTemplate<PurchaseButtonStatus>
        {
        }
    }
}