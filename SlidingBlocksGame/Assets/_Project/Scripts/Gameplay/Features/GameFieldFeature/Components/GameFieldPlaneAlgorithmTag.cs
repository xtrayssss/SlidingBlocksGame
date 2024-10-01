using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameFieldFeature.Components
{
    [Serializable]
    [MetaGroup("GameField")]
    public struct GameFieldPlaneAlgorithmTag : IEcsTagComponent
    {
		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Components", sourceClassName: "GameFieldPlaneAlgorithmTag/Template", sourceAssembly: "Assembly-CSharp")]
		private sealed class Template : TagComponentTemplate<GameFieldPlaneAlgorithmTag>
        {
        }
    }
}