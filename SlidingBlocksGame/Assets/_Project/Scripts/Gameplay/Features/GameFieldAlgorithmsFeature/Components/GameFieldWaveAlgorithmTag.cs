using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Components
{
    [Serializable]
    public struct GameFieldWaveAlgorithmTag : IEcsTagComponent
    {
        private sealed class Template : TagComponentTemplate<GameFieldWaveAlgorithmTag>
        {
        }
    }
}