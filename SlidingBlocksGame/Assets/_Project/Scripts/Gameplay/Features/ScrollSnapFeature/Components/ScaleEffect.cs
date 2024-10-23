using System;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components
{
    [Serializable]
    [MetaGroup("ScrollSnap")]
    public struct ScaleEffect : IEcsComponent
    {
        public Vector2 SelectedItemScale;
        public Vector2 UnselectedItemScale;

        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components",
            sourceClassName: "SelectionStateEffectFactor/Template", sourceAssembly: "ScrollSnapFeature.Components")]
        public sealed class Template : ComponentTemplate<ScaleEffect>
        {
        }
    }
}