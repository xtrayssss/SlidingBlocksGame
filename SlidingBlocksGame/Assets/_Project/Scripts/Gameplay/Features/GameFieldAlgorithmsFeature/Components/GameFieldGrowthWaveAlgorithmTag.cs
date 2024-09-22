using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Components
{
    [Serializable] 
    [MetaGroup("GameField")]
    public struct GameFieldGrowthWaveAlgorithmTag : IEcsTagComponent
    {
        private sealed class Template : TagComponentTemplate<GameFieldGrowthWaveAlgorithmTag>
        {
        }
    }
}