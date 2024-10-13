using System;
using DCFApixels.DragonECS;
using UnityEngine.Serialization;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct RewardStatus : IEcsComponent
    {
        [FormerlySerializedAs("Collected")] public EcsEntityConnect Locked;
        [FormerlySerializedAs("UnCollected")] public EcsEntityConnect Unlocked;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components", sourceClassName: "RewardStatus/Template", sourceAssembly: "Assembly-CSharp")]


		private sealed class Template : ComponentTemplate<RewardStatus>
        {
        }
    }
}