using System;
using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameProgressFeature.IntegrationFeatures.AudioFeature.Components
{
    [Serializable]
    [MetaGroup("GameProgress/Audio")]
    public struct CoinCollectedAudioConfig : IEcsAudioConfig
    {
        [field: SerializeField] public ScriptableEntityTemplate Value { get; set; }

        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.AudioBaseFeature.Components",
            sourceClassName: "CollectedAudioConfig/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<CoinCollectedAudioConfig>
        {
        }
    }
}