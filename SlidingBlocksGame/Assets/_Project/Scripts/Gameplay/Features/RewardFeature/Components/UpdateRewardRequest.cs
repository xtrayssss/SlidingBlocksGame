using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.RewardFeature.Components
{
    public struct UpdateRewardRequest : IEcsComponent
    {
        public long CollectionTime;
        public int Count;
        public bool Overwrite;
    }
}