using System;
using DCFApixels.DragonECS;
using Unity.Mathematics;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Components
{
    [Serializable] 
    [MetaGroup("Movement")]
    public struct WorldDestination : IEcsComponent
    {
        public float3 Value;

        private sealed class Template : ComponentTemplate<WorldDestination>
        {
        }
    }
}