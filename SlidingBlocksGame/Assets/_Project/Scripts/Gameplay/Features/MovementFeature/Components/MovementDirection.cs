using System;
using DCFApixels.DragonECS;
using Unity.Mathematics;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Components
{
    [Serializable]
    public struct MovementDirection : IEcsComponent
    {
        public float3 Value;

        [Serializable]
        public sealed class Template : ComponentTemplate<MovementDirection>
        {
        }
    }
}