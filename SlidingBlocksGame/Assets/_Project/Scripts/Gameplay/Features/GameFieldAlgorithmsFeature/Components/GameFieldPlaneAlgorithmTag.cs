using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Components
{
    [Serializable]
    [MetaGroup("GameField")]
    public struct GameFieldPlaneAlgorithmTag : IEcsTagComponent
    {
        private sealed class Template : TagComponentTemplate<GameFieldPlaneAlgorithmTag>
        {
        }
    }
}