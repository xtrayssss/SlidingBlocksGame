using System;
using _Project.Scripts.Gameplay.Features.AudioBaseFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.AudioFeature.Components
{
    [Serializable]
    [MetaGroup("Audio")]
    public struct RewardCoinDisplayCompletedAudioConfig : IEcsAudioConfig
    {
        [field: SerializeField] public ScriptableEntityTemplate Value { get; set; }

        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.AudioFeature.Components",
            sourceClassName: "RewardCollectedAudioConfig/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<RewardCoinDisplayCompletedAudioConfig>
        {
        }
    }
}