using System;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CameraFeature.Components
{
    [Serializable]
    [MetaGroup("Camera")]
    public struct PreviewCameraPrefab : IEcsComponent
    {
        public Camera Value;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.CameraFeature.Components",
            sourceClassName: "Renderer3DCameraPrefab/Template", sourceAssembly: "CameraFeature.Components")]
        private sealed class Template : ComponentTemplate<PreviewCameraPrefab>
        {
        }
    }
}