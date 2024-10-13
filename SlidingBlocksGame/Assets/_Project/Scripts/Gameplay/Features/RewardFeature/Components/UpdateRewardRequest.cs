using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.RewardFeature.Components
{
    public struct UpdateRewardRequest : IEcsComponent
    {
        public long Time;
        public int Count;
        public bool Overwrite;
    }
}