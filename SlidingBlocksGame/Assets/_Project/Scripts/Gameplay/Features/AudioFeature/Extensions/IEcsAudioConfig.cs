using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Extensions
{
    public interface IEcsAudioConfig : IEcsComponent
    {
        public ScriptableEntityTemplate Value { get; set; }
    }
}