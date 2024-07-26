using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Components
{
    [Serializable]
    public struct MovementEasingCfg : IEcsComponent
    {
        public ScriptableEntityTemplate Value;

        private sealed class Wrapper : ComponentTemplate<MovementEasingCfg>
        {
        }
    }
}