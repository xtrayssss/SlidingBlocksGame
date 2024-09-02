using System;
using DCFApixels.DragonECS;
using Unity.Mathematics;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct CellPosition : IEcsComponent
    {
        public int2 Value;

        [Serializable]
        public sealed class Wrapper : ComponentTemplate<CellPosition>
        {
        }   
    }
}