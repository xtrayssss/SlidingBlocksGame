using System;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.PurchaseFeature.Components
{
    [Serializable]
    public struct RotationTween : IEcsComponent
    {
        public Tween Value;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components",
            sourceClassName: "RotationTween/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<RotationTween>
        {
        }
    }
}