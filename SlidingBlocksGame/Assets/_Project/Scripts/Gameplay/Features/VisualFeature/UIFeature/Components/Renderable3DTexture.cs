using System;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components
{
    [Serializable]
    [MetaGroup("UI")]
    public struct Renderable3DTexture : IEcsComponent
    {
        public RenderTexture Value;

        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components",
            sourceClassName: "Renderable3DTexture/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<Renderable3DTexture>
        {
        }
    }
}