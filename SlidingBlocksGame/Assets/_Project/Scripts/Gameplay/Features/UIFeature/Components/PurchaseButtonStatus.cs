using System;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct PurchaseButtonStatus : IEcsComponent
    {
        public GameObject Lock;
        public GameObject Unlock;
        public GameObject Play;
        public GameObject Price;
        
        public GameObject Current;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components", sourceClassName: "PurchaseButtonStatus/Template", sourceAssembly: "Assembly-CSharp")]
		private sealed class Template : ComponentTemplate<PurchaseButtonStatus>
        {
        }
    }
}