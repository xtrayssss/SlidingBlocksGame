using System;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct MeshRendererRef : IEcsComponent
    {
        public MeshRenderer Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.CommonFeature.Components", sourceClassName: "MeshRendererRef/Template", sourceAssembly: "Assembly-CSharp")]
		private sealed class Template : ComponentTemplate<MeshRendererRef>
        {
        }
    }
}