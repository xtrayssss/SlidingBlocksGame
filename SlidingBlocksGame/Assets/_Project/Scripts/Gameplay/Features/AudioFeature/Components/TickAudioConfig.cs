using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Components
{
    [Serializable]
    public struct TickAudioConfig : IEcsComponent
    {
        public ScriptableEntityTemplate Value;
        
        private sealed class Template : ComponentTemplate<TickAudioConfig>
        {
        }
    }
}