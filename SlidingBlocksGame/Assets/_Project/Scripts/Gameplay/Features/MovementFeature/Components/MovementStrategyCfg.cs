using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Components
{
    [Serializable]    
    [MetaGroup("Movement")]
    public struct MovementStrategyCfg : IEcsComponent
    {
        public ScriptableEntityTemplate Value;

        private sealed class Template : ComponentTemplate<MovementStrategyCfg>
        {
        }
    }
}