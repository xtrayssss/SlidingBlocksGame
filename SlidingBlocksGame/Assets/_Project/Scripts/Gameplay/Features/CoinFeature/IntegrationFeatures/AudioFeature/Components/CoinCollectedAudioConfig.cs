using System;
using _Project.Scripts.Gameplay.Features.AudioFeature.Extensions;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CoinFeature.IntegrationFeatures.AudioFeature.Components
{
    [Serializable]
    [MetaGroup("Coin/Audio")]
    public struct CoinCollectedAudioConfig : IEcsAudioConfig
    {
        [field: SerializeField] public ScriptableEntityTemplate Value { get; set; }

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace:
            "_Project.Scripts.Gameplay.Features.GameProgressFeature.IntegrationFeatures.AudioFeature.Components",
            sourceClassName: "CoinCollectedAudioConfig/Template", sourceAssembly: "GameProgressFeature.AudioFeature.Components")]
        private sealed class Template : ComponentTemplate<CoinCollectedAudioConfig>
        {
        }
    }
}