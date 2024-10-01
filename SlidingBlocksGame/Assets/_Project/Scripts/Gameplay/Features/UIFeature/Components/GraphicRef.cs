using System;
using DCFApixels.DragonECS;
using UnityEngine.UI;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct GraphicRef : IEcsComponent
    {
        public Graphic Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components", sourceClassName: "GraphicRef/Template", sourceAssembly: "Assembly-CSharp")]
		private sealed class Template : ComponentTemplate<GraphicRef>
        {
        }
    }
}