using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.ScoreFeature.Components
{
    public struct BestScoreUpdatedEvent : IEcsComponent
    {
        public int Delta;
    }
}