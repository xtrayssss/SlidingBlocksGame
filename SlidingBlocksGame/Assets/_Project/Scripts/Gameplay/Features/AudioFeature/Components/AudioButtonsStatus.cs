using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Components
{
    [Serializable]
    public struct AudioButtonsStatus : IEcsComponent
    {
        public bool MusicIsOn;
        public bool SoundIsOn;

        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components",
            sourceClassName: "AudioButtonsStatus/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<AudioButtonsStatus>
        {
        }
    }
}