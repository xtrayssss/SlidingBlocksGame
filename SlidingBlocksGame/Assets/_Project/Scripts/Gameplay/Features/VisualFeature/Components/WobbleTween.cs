using System;
using DCFApixels.DragonECS;
using PrimeTween;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.Components
{
    [Serializable]
    public struct WobbleTween : IEcsComponent
    {
        public Tween Value;
    }
}