using System;
using System.Threading.Tasks;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;


namespace _Project.Scripts.Gameplay.Features.GameFieldFeature.Components
{
    [Serializable]
    public struct GrowthWave : IEcsComponent
    {
        public Task[] GrowthTasks;
        public Task[] ShrinkTasks;
        
        [FormerlySerializedAs("Base")] public float BaseSpeedFactor;
        [FormerlySerializedAs("Speed")] public float SpeedFactor;

        [Serializable]

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components", sourceClassName: "GrowthWave/Wrapper", sourceAssembly: "Assembly-CSharp")]


		public sealed class Wrapper : ComponentTemplate<GrowthWave>
        {
        }
    }
}