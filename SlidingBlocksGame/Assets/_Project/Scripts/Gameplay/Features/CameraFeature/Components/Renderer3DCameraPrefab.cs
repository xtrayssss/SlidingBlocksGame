using System;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CameraFeature.Components
{
    [Serializable]
    [MetaGroup("Camera")]
    public struct Renderer3DCameraPrefab : IEcsComponent
    {
        public Camera Value;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components",
            sourceClassName: "Renderer3DCameraPrefab/Template", sourceAssembly: "UIFeature.Components")]
        private sealed class Template : ComponentTemplate<Renderer3DCameraPrefab>
        {
        }
    }
}