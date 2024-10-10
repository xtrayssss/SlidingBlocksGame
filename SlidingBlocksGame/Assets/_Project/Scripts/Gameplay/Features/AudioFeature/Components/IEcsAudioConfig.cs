using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Components
{
    public interface IEcsAudioConfig : IEcsComponent
    {
        public ScriptableEntityTemplate Value { get; set; }
    }
}