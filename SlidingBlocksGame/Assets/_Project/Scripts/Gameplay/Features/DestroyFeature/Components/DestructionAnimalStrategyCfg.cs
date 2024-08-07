using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.DestroyFeature.Components
{
    [Serializable]
    public struct DestructionAnimalStrategyCfg : IEcsComponent
    {
        public ScriptableEntityTemplate Value;

        private sealed class Template : ComponentTemplate<DestructionAnimalStrategyCfg>
        {
        }
    }
}