using System;
using System.Collections.Generic;
using DCFApixels.DragonECS;
using Unity.Mathematics;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Components
{
    [Serializable]
    public struct ProcessedSides : IEcsComponent
    {
        public List<int2> Value;
    }
}