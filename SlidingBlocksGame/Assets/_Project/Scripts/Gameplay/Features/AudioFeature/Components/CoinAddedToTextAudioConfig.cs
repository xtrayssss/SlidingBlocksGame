using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Components
{
    [Serializable]
    [MetaGroup("Audio")]
    public struct CoinAddedToTextAudioConfig : IEcsComponent
    {
        public ScriptableEntityTemplate Value;
        
        private sealed class Template : ComponentTemplate<CoinAddedToTextAudioConfig>
        {
        }
    }
}