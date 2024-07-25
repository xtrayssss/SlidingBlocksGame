using System;
using DCFApixels.DragonECS;
using UnityEngine.Serialization;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components
{
    [Serializable]
    public struct AlgorithmGenerationGameField : IEcsComponent
    {
        public ID Value;
        
        public enum ID
        {
            None,
            Wave,
            SmoothnessWave,
            Random
        }

        [Serializable]
        public sealed class Wrapper : ComponentTemplate<AlgorithmGenerationGameField>
        {
        }
    }
}