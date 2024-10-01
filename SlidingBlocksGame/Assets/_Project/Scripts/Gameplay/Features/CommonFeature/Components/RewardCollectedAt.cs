using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct RewardCollectedAt : IEcsComponent
    {
        public long Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.CommonFeature.Components", sourceClassName: "RewardCollectedAt/Template", sourceAssembly: "Assembly-CSharp")]
		private sealed class Template : ComponentTemplate<RewardCollectedAt>
        {
        }
    }
}