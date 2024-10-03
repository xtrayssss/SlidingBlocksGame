using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.AudioBaseFeature.Components
{
    [Serializable]
    [MetaGroup("Audio")]
    public struct VolumeEffectTag : IEcsTagComponent
    {

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.AudioFeature.Components", sourceClassName: "VolumeEffectTag/Template", sourceAssembly: "Assembly-CSharp")]


		private sealed class Template : TagComponentTemplate<VolumeEffectTag>
        {
        }
    }
}