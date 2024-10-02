using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    public struct BestScoreUpdatedEvent : IEcsComponent
    {
        public int Delta;
    }
}