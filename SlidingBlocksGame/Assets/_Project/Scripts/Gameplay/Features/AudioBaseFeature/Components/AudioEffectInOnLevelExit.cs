using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.AudioBaseFeature.Components
{
    [Serializable]
    [MetaGroup("Audio")]
    public struct AudioEffectInOnLevelExit : IEcsComponent
    {
        public EntityTemplate Value;

        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.AudioFeature.Components",
            sourceClassName: "AudioEffectInOnLevelExit/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<AudioEffectInOnLevelExit>
        {
        }
    }
}