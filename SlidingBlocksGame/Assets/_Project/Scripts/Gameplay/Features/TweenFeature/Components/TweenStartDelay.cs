using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.TweenFeature.Components
{
    [Serializable]
    [MetaGroup("Tween")]
    public struct TweenStartDelay : IEcsComponent
    {
        public float Value;

        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.TweenFeature.Components",
            sourceClassName: "TweenStartDelay/Template", sourceAssembly: "TweenFeature.Components")]
        private sealed class Template : ComponentTemplate<TweenStartDelay>
        {
        }
    }
}