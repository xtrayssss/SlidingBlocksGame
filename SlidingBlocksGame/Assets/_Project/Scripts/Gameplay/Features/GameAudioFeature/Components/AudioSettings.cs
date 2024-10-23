using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameAudioFeature.Components
{
    [Serializable]
    [MetaGroup("GameAudio")]
    public struct AudioSettings : IEcsComponent
    {
        public bool MusicIsOn;
        public bool SoundIsOn;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.GameAudioFeature.Components",
            sourceClassName: "AudioButtonsStatus/Template", sourceAssembly: "GameAudioFeature.Components")]
        private sealed class Template : ComponentTemplate<AudioSettings>
        {
        }
    }
}