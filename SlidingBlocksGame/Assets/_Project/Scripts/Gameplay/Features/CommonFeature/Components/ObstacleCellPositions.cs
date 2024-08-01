using System.Collections.Generic;
using DCFApixels.DragonECS;
using Unity.Mathematics;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    public struct ObstacleCellPositions : IEcsComponent
    {
        public List<float2> Value;
    }
}