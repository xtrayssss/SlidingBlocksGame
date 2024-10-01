using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct BestScore : IEcsComponent
    {
        public int Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components", sourceClassName: "BestScore/Template", sourceAssembly: "Assembly-CSharp")]
		private sealed class Template : ComponentTemplate<BestScore>
        {
        }
    }
}