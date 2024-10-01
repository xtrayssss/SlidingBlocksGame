using System;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Components
{
    [Serializable]
    [MetaGroup("Audio")]
    public struct Audio : IEcsComponent
    {
        public AudioClip Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.AudioFeature.Components", sourceClassName: "Audio/Template", sourceAssembly: "Assembly-CSharp")]
		private sealed class Template : ComponentTemplate<Audio>
        {
        }
    }
}