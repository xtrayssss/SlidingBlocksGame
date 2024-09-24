using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Components
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

        private sealed class Template : ComponentTemplate<AudioTypeRef>
        {
        }
    }
}