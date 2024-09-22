using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Components
{
    [Serializable]
    [MetaGroup("Audio")]
    public struct AudioEffectInOnLevelEnter : IEcsComponent
    {
        public EntityTemplate Value;

        private sealed class Template : ComponentTemplate<AudioEffectInOnLevelEnter>
        {
        }
    }
}