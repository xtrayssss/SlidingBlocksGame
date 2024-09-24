using System;
using System.Threading.Tasks;
using DCFApixels.DragonECS;
using UnityEngine.Serialization;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components
{
    [Serializable]
    public struct GrowthWave : IEcsComponent
    {
        public Task[] GrowthTasks;
        public Task[] ShrinkTasks;
        
        [FormerlySerializedAs("Base")] public float BaseSpeedFactor;
        [FormerlySerializedAs("Speed")] public float SpeedFactor;

        [Serializable]
        public sealed class Wrapper : ComponentTemplate<GrowthWave>
        {
        }
    }
}