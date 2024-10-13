using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameProgressFeature.Components
{
    public struct UpdateGameAudioRequest : IEcsComponent
    {
        public bool IsMusicOn;
        public bool IsSoundOn;
    }
}