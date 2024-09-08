using System;
using DCFApixels.DragonECS;
using Unity.Mathematics;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components
{
    [Serializable]
    public struct Wave : IEcsComponent
    {
        public float Speed;
        public float3 WaveOrigin;

        [Serializable]
        public sealed class Wrapper : ComponentTemplate<Wave>
        {
        }
    }
}