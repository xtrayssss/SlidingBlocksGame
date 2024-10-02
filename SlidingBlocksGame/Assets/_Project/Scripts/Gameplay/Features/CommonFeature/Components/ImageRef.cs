using System;
using DCFApixels.DragonECS;
using UnityEngine.UI;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct ImageRef : IEcsComponent
    {
        public Image Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.CommonFeature.Components", sourceClassName: "ImageRef/Template", sourceAssembly: "Assembly-CSharp")]


		private sealed class Template : ComponentTemplate<ImageRef>
        {
        }
    }
}