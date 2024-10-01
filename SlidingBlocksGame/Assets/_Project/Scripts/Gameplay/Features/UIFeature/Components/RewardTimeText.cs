using System;
using DCFApixels.DragonECS;
using TMPro;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct RewardTimeText : IEcsComponent
    {
        public TextMeshProUGUI Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components", sourceClassName: "RewardTimeText/Template", sourceAssembly: "Assembly-CSharp")]
		private sealed class Template : ComponentTemplate<RewardTimeText>
        {
        }
    }
}