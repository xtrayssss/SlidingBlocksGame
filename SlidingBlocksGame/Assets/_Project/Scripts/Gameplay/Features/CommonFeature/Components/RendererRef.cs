using System;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct RendererRef : IEcsComponent
    {
        public GameObject Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.CommonFeature.Components", sourceClassName: "RendererRef/Template", sourceAssembly: "Assembly-CSharp")]


		private class Template : ComponentTemplate<RendererRef>
        {
            
        }
    }
}