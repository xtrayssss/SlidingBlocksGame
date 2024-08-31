using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Components
{
    [Serializable]
    public struct GameFieldGeneratedWaveAlgorithmAudioConfig : IEcsComponent
    {
        public ScriptableEntityTemplate Value;

        private sealed class Template : ComponentTemplate<GameFieldGeneratedWaveAlgorithmAudioConfig>
        {
        }
    }
}