using System;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

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

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components", sourceClassName: "InAppPurchases/Purchase/Template", sourceAssembly: "Assembly-CSharp")]
		private sealed class Template : ComponentTemplate<InAppPurchases>
        {
        }
    }
}