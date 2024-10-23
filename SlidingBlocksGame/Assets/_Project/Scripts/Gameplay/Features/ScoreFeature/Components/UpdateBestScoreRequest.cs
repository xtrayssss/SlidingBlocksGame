using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.ScoreFeature.Components
{
    public struct UpdateBestScoreRequest : IEcsComponent
    {
        public bool Overwrite;
        public int Value;
    }
}