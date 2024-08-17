using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct InGamePurchaseAnimals : IEcsComponent
    {
        public EcsGroup Value;
        public GameObject[] Proto;

        private sealed class Template : ComponentTemplate<InGamePurchaseAnimals>
        {
        }
    }
}