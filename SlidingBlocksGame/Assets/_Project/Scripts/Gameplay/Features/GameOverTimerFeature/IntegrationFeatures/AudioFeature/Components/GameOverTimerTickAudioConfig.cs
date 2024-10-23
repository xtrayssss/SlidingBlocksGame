using System;
using _Project.Scripts.Gameplay.Features.AudioFeature.Extensions;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameOverTimerFeature.IntegrationFeatures.AudioFeature.Components
{
    [Serializable]
    [MetaGroup("GameOverTimer/Audio")]
    public struct GameOverTimerTickAudioConfig : IEcsAudioConfig
    {
        [field: SerializeField] public ScriptableEntityTemplate Value { get; set; }

        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.AudioFeature.Components",
            sourceClassName: "TickAudioConfig/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<GameOverTimerTickAudioConfig>
        {
        }
    }
}