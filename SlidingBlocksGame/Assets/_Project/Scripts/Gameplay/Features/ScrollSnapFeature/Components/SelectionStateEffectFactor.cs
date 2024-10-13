using System;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components
{
    [Serializable]
    [MetaGroup("ScrollSnap")]
    public struct SelectionStateEffectFactor : IEcsComponent
    {
        public float3 Selected;
        public float3 Deselected;

        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components",
            sourceClassName: "SelectionStateEffectFactor/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<SelectionStateEffectFactor>
        {
        }
    }
}