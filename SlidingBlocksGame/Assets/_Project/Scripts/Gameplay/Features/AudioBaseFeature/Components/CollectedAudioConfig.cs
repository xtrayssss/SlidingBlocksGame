using System;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.AudioBaseFeature.Components
{
    [Serializable]
    [MetaGroup("Audio")]
    public struct CollectedAudioConfig : IEcsAudioConfig
    {
        [field: SerializeField] public ScriptableEntityTemplate Value { get; set; }

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.AudioFeature.Components", sourceClassName: "CollectedAudioConfig/Template", sourceAssembly: "Assembly-CSharp")]


		private sealed class Template : ComponentTemplate<CollectedAudioConfig>
        {
        }
    }
}