using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.InputFeature.Components
{
    public struct ClickDownEvent : IEcsTagComponent
    {
    }
    
    public struct ScreenPosition : IEcsComponent
    {
        public float2 Value;
    }
}