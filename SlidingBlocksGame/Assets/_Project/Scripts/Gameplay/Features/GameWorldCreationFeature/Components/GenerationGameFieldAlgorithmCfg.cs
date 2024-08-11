using System;
using DCFApixels.DragonECS;
using UnityEngine.Serialization;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components
{
    [Serializable]
    public struct GenerationGameFieldAlgorithmCfg : IEcsComponent
    {
        public ScriptableEntityTemplate Value;
        
        public sealed class Template : ComponentTemplate<GenerationGameFieldAlgorithmCfg>
        {
        }
    }
}