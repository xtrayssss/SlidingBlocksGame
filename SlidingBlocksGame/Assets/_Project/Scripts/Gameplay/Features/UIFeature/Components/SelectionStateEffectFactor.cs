using System;
using DCFApixels.DragonECS;
using Unity.Mathematics;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct SelectionStateEffectFactor : IEcsComponent
    {
        public float3 Selected;
        public float3 Deselected;

        private sealed class Template : ComponentTemplate<SelectionStateEffectFactor>
        {
        }
    }
}