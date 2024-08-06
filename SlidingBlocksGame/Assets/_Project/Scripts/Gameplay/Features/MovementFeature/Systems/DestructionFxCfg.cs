using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    [Serializable]
    public struct DestructionFxCfg : IEcsComponent
    {
        public ScriptableEntityTemplate Value;

        private class Template : ComponentTemplate<DestructionFxCfg>
        {
        }
    }
}