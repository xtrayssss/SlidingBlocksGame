using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components
{
    [Serializable]
    public struct GraphicRef : IEcsComponent
    {
        public Graphic Value;

        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components",
            sourceClassName: "GraphicRef/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<GraphicRef>
        {
        }
    }
}