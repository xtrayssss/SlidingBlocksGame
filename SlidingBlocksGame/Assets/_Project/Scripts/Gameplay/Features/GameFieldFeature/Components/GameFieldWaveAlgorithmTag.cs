using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameFieldFeature.Components
{
    [Serializable]
    [MetaGroup("GameField")]
    public struct GameFieldWaveAlgorithmTag : IEcsTagComponent
    {
		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.GameFieldFeature.Components", sourceClassName: "GameFieldWaveAlgorithmTag/Template", sourceAssembly: "Assembly-CSharp")]

		private sealed class Template : TagComponentTemplate<GameFieldWaveAlgorithmTag>
        {
        }
    }
}