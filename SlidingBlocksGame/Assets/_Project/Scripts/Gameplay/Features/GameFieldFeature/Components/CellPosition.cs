using System;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameFieldFeature.Components
{
    [Serializable]
    [MetaGroup("GameField")]
    public struct CellPosition : IEcsComponent
    {
        public int2 Value;

        [Serializable]
        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.CommonFeature.Components",
            sourceClassName: "CellPosition/Wrapper", sourceAssembly: "Assembly-CSharp")]
        public sealed class Wrapper : ComponentTemplate<CellPosition>
        {
        }
    }
}