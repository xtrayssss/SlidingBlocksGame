using System;
using DCFApixels.DragonECS;
using Unity.Mathematics;

namespace _Project.Scripts.Gameplay.Features.EasingFeature.Components
{
    [Serializable]
    public struct EasingDestination : IEcsComponent
    {
        public float3 Original;
        public float3 Destination;
        public float3 Interpolation;

        [Serializable]
        public sealed class Wrapper : ComponentTemplate<EasingDestination>
        {
        }
    }
}