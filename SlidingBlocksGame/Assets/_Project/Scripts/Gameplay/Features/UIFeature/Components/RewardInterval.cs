using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct RewardInterval : IEcsComponent
    {
        public long Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components", sourceClassName: "RewardInterval/Template", sourceAssembly: "Assembly-CSharp")]


		private sealed class Template : ComponentTemplate<RewardInterval>
        {
        }
    }
}