using System;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.RewardFeature.Components
{
    [Serializable]
    [MetaGroup("Reward")]
    public struct RewardScalingCurve : IEcsComponent
    {
        public AnimationCurve Value;

        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.RewardFeature.Components",
            sourceClassName: "RewardScalingCurve/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<RewardScalingCurve>
        {
        }
    }
}