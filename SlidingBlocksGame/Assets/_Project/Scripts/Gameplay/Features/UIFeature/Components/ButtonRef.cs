using System;
using DCFApixels.DragonECS;
using UnityEngine.UI;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct ButtonRef : IEcsComponent
    {
        public Button Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components", sourceClassName: "ButtonRef/Template", sourceAssembly: "Assembly-CSharp")]
		private sealed class Template : ComponentTemplate<ButtonRef>
        {
        }
    }
}