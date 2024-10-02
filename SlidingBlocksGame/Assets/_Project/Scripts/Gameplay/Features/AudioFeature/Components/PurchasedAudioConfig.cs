using System;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Components
{
    [Serializable]
    [MetaGroup("Audio")]
    public struct PurchasedAudioConfig : IEcsAudioConfig
    {
        [field: SerializeField] public ScriptableEntityTemplate Value { get; set; }

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.AudioFeature.Components", sourceClassName: "PurchasedAudioConfig/Template", sourceAssembly: "Assembly-CSharp")]


		private sealed class Template : ComponentTemplate<PurchasedAudioConfig>
        {
        }
    }
}