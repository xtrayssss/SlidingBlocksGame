using System;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    [MetaGroup("Common")]
    public struct RendererRef : IEcsComponent
    {
        public Renderer Value;

        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.CommonFeature.Components",
            sourceClassName: "MeshRendererRef/Template", sourceAssembly: "CommonFeature.Components")]
        private sealed class Template : ComponentTemplate<RendererRef>
        {
        }
    }
}