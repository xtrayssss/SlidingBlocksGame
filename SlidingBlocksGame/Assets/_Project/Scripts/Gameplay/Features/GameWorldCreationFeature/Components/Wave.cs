using System;
using DCFApixels.DragonECS;
using Unity.Mathematics;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components
{
    [Serializable]
    public struct Wave : IEcsComponent
    {
        public float SpeedFactor;
        public float3 WaveOrigin;
        public float BaseSpeedFactor;

        [Serializable]
        public sealed class Wrapper : ComponentTemplate<Wave>
        {
        }
    }
}