using System;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.AudioBaseFeature.Components
{
    [Serializable]
    [MetaGroup("Audio")]
    public struct AudioSourceRef : IEcsComponent
    {
        public AudioSource Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.AudioFeature.Components", sourceClassName: "AudioSourceRef/Template", sourceAssembly: "Assembly-CSharp")]


		private sealed class Template : ComponentTemplate<AudioSourceRef>
        {
        }
    }
}