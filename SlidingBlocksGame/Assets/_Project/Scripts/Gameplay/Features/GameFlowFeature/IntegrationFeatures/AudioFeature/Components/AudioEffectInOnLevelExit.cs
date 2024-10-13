using System;
using _Project.Scripts.Gameplay.Templates;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.IntegrationFeatures.AudioFeature.Components
{
    [Serializable]
    [MetaGroup("Audio")]
    public struct AudioEffectInOnLevelExit : IEcsComponent
    {
        public EntityTemplate Value;

        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.GameFlowFeature.IntegrationFeatures.AudioFeature.Components",
            sourceClassName: "AudioEffectInOnLevelExit/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<AudioEffectInOnLevelExit>
        {
        }
    }
}