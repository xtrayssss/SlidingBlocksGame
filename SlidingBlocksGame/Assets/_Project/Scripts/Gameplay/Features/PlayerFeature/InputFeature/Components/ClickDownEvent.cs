using DCFApixels.DragonECS;
using Unity.Mathematics;

namespace _Project.Scripts.Gameplay.Features.PlayerFeature.InputFeature.Components
{
    public struct ClickDownEvent : IEcsTagComponent
    {
    }
    
    public struct ScreenPosition : IEcsComponent
    {
        public float2 Value;
    }
}