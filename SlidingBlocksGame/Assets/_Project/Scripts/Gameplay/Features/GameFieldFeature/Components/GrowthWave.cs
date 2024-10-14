using System;
using _Project.Scripts.DragonAPI;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;


namespace _Project.Scripts.Gameplay.Features.GameFieldFeature.Components
{
    [Serializable]
    [MetaGroup("GameField/CellPosition")]
    public struct GrowthWave : IEcsComponent
    {
        public DragonCoroutine[] GrowthCoroutines;
        public DragonCoroutine[] ShrinkCoroutines;
        
        [FormerlySerializedAs("Base")] public float BaseSpeedFactor;
        [FormerlySerializedAs("Speed")] public float SpeedFactor;

        [Serializable]

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components", sourceClassName: "GrowthWave/Wrapper", sourceAssembly: "Assembly-CSharp")]


		public sealed class Wrapper : ComponentTemplate<GrowthWave>
        {
        }
    }
}