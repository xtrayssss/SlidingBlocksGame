using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Components
{
    [Serializable]
    [MetaGroup("Audio")]
    public struct AudioTypeRef : IEcsComponent
    {
        public Type Value;

        public enum Type : byte
        {
            NONE = 0,
            SFX_NORMAL = 1,
            SFX_SPECIAL = 2,
            MUSIC = 3
        }

        private sealed class Template : ComponentTemplate<AudioTypeRef>
        {
        }
    }
}