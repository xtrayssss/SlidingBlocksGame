using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.AudioBaseFeature.Components
{
    [Serializable]
    [MetaGroup("Audio")]
    public struct AudioTypeRef : IEcsComponent
    {
        public Type Value;

        [Flags]
        public enum Type : byte
        {
            NONE = 0,
            SFX_NORMAL = 1 << 0,
            SFX_SPECIAL = 1 << 1,
            MUSIC = 1 << 2,
            
            SFX = SFX_NORMAL | SFX_SPECIAL
        }

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.AudioFeature.Components", sourceClassName: "AudioTypeRef/Template", sourceAssembly: "Assembly-CSharp")]


		private sealed class Template : ComponentTemplate<AudioTypeRef>
        {
        }
    }
}