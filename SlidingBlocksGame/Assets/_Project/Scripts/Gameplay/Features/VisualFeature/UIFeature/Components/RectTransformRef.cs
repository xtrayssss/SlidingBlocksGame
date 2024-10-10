using System;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components
{
    [Serializable]
    public struct RectTransformRef : IEcsComponent
    {
        public RectTransform Value;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components",
            sourceClassName: "RectTransformRef/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<RectTransformRef>
        {
        }
    }
}