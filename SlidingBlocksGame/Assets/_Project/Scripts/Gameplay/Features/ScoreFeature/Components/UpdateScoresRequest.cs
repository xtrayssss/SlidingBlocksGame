using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.ScoreFeature.Components
{
    public struct UpdateScoresRequest : IEcsComponent
    {
        public int Value;
        public bool Overwrite;
    }
}