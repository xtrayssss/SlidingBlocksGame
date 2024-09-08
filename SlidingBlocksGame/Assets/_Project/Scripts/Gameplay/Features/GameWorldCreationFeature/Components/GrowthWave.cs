using System;
using System.Threading.Tasks;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components
{
    [Serializable]
    public struct GrowthWave : IEcsComponent
    {
        public Task[] GrowthTasks;
        public Task[] ShrinkTasks;
        
        public float GrowthDuration;

        [Serializable]
        public sealed class Wrapper : ComponentTemplate<GrowthWave>
        {
        }
    }
}