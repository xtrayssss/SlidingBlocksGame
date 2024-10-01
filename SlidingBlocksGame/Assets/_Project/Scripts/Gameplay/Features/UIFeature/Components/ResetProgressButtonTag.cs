using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct ResetProgressButtonTag : IEcsTagComponent
    {

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components", sourceClassName: "ResetProgressButtonTag/Template", sourceAssembly: "Assembly-CSharp")]
		public class Template : TagComponentTemplate<ResetProgressButtonTag>
        {
        }
    }
}