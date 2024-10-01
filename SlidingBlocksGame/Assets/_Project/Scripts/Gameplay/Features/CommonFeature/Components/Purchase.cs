using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct Purchase : IEcsComponent
    {
        public int Price;
        public ushort ProductIndex;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.CommonFeature.Components", sourceClassName: "Purchase/Template", sourceAssembly: "Assembly-CSharp")]
		private sealed class Template : ComponentTemplate<Purchase>
        {
        }
    }
}