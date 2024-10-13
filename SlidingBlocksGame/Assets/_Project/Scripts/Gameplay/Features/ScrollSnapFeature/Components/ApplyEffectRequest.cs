using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components
{
    public struct ApplyEffectRequest : IEcsComponent
    {
        public EcsGroup Items;
        public float Ratio;
    }
}