using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct RewardsCount : IEcsComponent
    {
        public int Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.CommonFeature.Components", sourceClassName: "RewardsCount/Template", sourceAssembly: "Assembly-CSharp")]
		private sealed class Template : ComponentTemplate<RewardsCount>
        {
        }
    }
}