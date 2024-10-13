using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components
{
    public struct SetupScrollRequest : IEcsComponent
    {
        public EcsGroup Items;
        public int ScrollToIndex;
        public bool IsAutoScroll;
    }
}