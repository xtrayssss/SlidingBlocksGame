using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameProgressFeature.Components
{
    public struct BestScoreUpdatedEvent : IEcsComponent
    {
        public int Delta;
    }
}