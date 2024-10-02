using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct GameLossTimerTag : IEcsTagComponent
    {

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.CommonFeature.Components", sourceClassName: "GameLossTimerTag/Template", sourceAssembly: "Assembly-CSharp")]


		private sealed class Template : TagComponentTemplate<GameLossTimerTag>
        {
        }
    }
}