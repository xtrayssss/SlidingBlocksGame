using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameFieldFeature.Components
{
    [Serializable] 
    [MetaGroup("GameField")]
    public struct GameFieldGrowthWaveAlgorithmTag : IEcsTagComponent
    {
		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Components", sourceClassName: "GameFieldGrowthWaveAlgorithmTag/Template", sourceAssembly: "Assembly-CSharp")]

		private sealed class Template : TagComponentTemplate<GameFieldGrowthWaveAlgorithmTag>
        {
        }
    }
}