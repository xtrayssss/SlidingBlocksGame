using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameAudioFeature.Components
{
    public struct UpdateAudioSettingsRequest : IEcsComponent
    {
        public bool IsMusicOn;
        public bool IsSoundOn;
    }
}