using DCFApixels.DragonECS;
using PrimeTween;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components
{
    public struct ScrollToTargetState : IEcsComponent
    {
        public Sequence ScrollTween;
        public entlong Delay;
        public bool IsAutoScroll;
    }
}