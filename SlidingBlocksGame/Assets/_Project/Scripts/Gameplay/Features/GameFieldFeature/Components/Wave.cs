using System;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameFieldFeature.Components
{
    [Serializable]
    [MetaGroup("GameField")]
    public struct Wave : IEcsComponent
    {
        public float SpeedFactor;
        public float3 WaveOrigin;
        public float BaseSpeedFactor;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components",
            sourceClassName: "Wave/Wrapper", sourceAssembly: "Assembly-CSharp")]
        public sealed class Wrapper : ComponentTemplate<Wave>
        {
        }
    }
}