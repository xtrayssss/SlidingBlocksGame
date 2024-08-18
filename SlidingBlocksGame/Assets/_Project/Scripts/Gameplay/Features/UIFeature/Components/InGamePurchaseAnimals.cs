using System;
using DCFApixels.DragonECS;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct InGamePurchaseAnimals : IEcsComponent
    {
        public EcsGroup Value;
        public GameObject[] Proto;
        public GameObject Purchase;

        private sealed class Template : ComponentTemplate<InGamePurchaseAnimals>
        {
        }
    }
}