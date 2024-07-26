using System;
using DCFApixels.DragonECS;
using Unity.Mathematics;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Components
{
    [Serializable]
    public struct Destination : IEcsComponent
    {
        public float3 Value;

        private sealed class Template : ComponentTemplate<Destination>
        {
        }
    }
}