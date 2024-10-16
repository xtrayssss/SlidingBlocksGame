using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameOverTimerFeature.Components
{
    [Serializable]
    [MetaGroup("GameOverTimer")]
    public struct GameOverTimerConfig : IEcsComponent
    {
        public ScriptableEntityTemplate Value;

        private sealed class Template : ComponentTemplate<GameOverTimerConfig>
        {
        }
    }
}