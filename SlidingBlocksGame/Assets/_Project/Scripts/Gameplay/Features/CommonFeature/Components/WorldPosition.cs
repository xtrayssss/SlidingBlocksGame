using System;
using DCFApixels.DragonECS;
using Unity.Mathematics;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct WorldPosition : IEcsComponent
    {
        public float3 Value;

        [Serializable]
        public sealed class Wrapper : ComponentTemplate<WorldPosition>
        {
        }
    }
}