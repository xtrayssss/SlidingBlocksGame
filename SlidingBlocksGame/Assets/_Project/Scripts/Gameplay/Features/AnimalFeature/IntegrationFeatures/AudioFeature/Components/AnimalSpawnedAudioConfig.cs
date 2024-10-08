using System;
using _Project.Scripts.Gameplay.Features.AudioBaseFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.AudioFeature.Components
{
    [Serializable]
    [MetaGroup("Animal")]
    public struct AnimalSpawnedAudioConfig : IEcsAudioConfig
    {
        [field: SerializeField] public ScriptableEntityTemplate Value { get; set; }

        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.AnimalFeature.Components",
            sourceClassName: "AnimalSpawnedAudioConfig/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<AnimalSpawnedAudioConfig>
        {
        }
    }
}