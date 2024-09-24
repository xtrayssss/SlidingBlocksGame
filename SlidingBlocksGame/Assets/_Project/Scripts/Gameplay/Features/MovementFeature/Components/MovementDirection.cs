using System;
using DCFApixels.DragonECS;
using Unity.Mathematics;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Components
{
    [Serializable]
    [MetaGroup("Movement")]
    public struct MovementDirection : IEcsComponent
    {
        public int2 Value;

        [Serializable]
        public sealed class Template : ComponentTemplate<MovementDirection>
        {
        }
    }
}