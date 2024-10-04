using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.PurchaseFeature.Components
{
    [Serializable]
    [MetaGroup("Purchase")]
    public struct Purchase : IEcsComponent
    {
        public int Price;
        public ushort Index;

        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.CommonFeature.Components",
            sourceClassName: "Purchase/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<Purchase>
        {
        }
    }
}