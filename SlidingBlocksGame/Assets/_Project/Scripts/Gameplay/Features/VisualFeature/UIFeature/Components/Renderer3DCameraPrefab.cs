using System;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components
{
    [Serializable]
    [MetaGroup("UI")]
    public struct Renderer3DCameraPrefab : IEcsComponent
    {
        public Camera Value;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components",
            sourceClassName: "Renderer3DCameraPrefab/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<Renderer3DCameraPrefab>
        {
        }
    }
}