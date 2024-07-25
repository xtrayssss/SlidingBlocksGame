using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components
{
    [Serializable]
    public struct WaveAlgorithm : IEcsComponent
    {
        public float Speed;

        [Serializable]
        public sealed class Wrapper : ComponentTemplate<WaveAlgorithm>
        {
        }

    }
}