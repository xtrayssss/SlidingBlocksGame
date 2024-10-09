using System;
using _Project.Scripts.Gameplay.Features.AudioBaseFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.ButtonFeature.IntegrationFeatures.AudioFeature.Components
{
    [Serializable]
    [MetaGroup("Button/Audio")]
    public struct ButtonClickedAudioConfig : IEcsAudioConfig
    {
        [field: SerializeField] public ScriptableEntityTemplate Value { get; set; }

        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.AudioFeature.Components",
            sourceClassName: "ClickedAudioConfig/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<ButtonClickedAudioConfig>
        {
        }
    }
}