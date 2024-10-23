using System;
using _Project.Scripts.Gameplay.Features.AudioFeature.Extensions;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.AudioFeature.Components
{
    [Serializable]
    [MetaGroup("Reward/Audio")]
    public struct ConfettiExplodedAudioConfig : IEcsAudioConfig
    {
        [field: SerializeField] public ScriptableEntityTemplate Value { get; set; }

        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.AudioBaseFeature.Components",
            sourceClassName: "ConfettiExplodedAudioConfig/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<ConfettiExplodedAudioConfig>
        {
        }
    }
}