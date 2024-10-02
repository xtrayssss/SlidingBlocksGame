using System;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.Components
{
    [Serializable]
    public struct CanvasRef : IEcsComponent
    {
        public Canvas Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.VisualFeature.Components", sourceClassName: "CanvasRef/Template", sourceAssembly: "Assembly-CSharp")]


		private sealed class Template : ComponentTemplate<CanvasRef>
        {
        }
    }
}