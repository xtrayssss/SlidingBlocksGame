using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components
{
    [Serializable]
    public struct GameLossTimerCfg : IEcsComponent
    {
        public ScriptableEntityTemplate Value;

        private sealed class Template : ComponentTemplate<GameLossTimerCfg>
        {
        }
    }
}