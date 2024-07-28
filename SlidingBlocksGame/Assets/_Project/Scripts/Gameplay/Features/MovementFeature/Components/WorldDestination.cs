using System;
using DCFApixels.DragonECS;
using Unity.Mathematics;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Components
{
    [Serializable]
    public struct WorldDestination : IEcsComponent
    {
        public float3 Value;

        private sealed class Template : ComponentTemplate<WorldDestination>
        {
        }
    }
}