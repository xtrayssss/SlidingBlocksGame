using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    [Serializable]
    public struct DestructionFxPrefab : IEcsComponent
    {
        public EcsEntityConnect Value;

        private class Template : ComponentTemplate<DestructionFxPrefab>
        {
        }
    }
}