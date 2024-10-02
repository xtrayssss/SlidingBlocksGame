using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.Components
{
    [Serializable]
    public struct GameTag : IEcsTagComponent
    {
        [Serializable]

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.GameFlowFeature.Components", sourceClassName: "GameTag/Template", sourceAssembly: "Assembly-CSharp")]


		public sealed class Template : TagComponentTemplate<GameTag>
        {
        }
    }
}