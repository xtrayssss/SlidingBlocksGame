using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components
{
    [Serializable]
    public struct WaveSmoothnessAlgorithm : IEcsComponent
    {
        public float GrowthDuration;

        [Serializable]
        public sealed class Wrapper : ComponentTemplate<WaveSmoothnessAlgorithm>
        {
        }
    }
}