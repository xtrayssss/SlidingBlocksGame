using System;
using DCFApixels.DragonECS;
using TMPro;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct GrabRewardText : IEcsComponent
    {
        public TextMeshProUGUI Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components", sourceClassName: "GrabRewardText/Template", sourceAssembly: "Assembly-CSharp")]


		private sealed class Template : ComponentTemplate<GrabRewardText>
        {
        }
    }
}