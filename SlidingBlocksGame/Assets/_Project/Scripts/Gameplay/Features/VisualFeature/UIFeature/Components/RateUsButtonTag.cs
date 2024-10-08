using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components
{
    [Serializable]
    public struct RateUsButtonTag : IEcsTagComponent
    {
        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components",
            sourceClassName: "RateUsButtonTag/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : TagComponentTemplate<RateUsButtonTag>
        {
        }
    }
}