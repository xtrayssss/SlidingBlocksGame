using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Components
{
    [Serializable]
    [MetaGroup("Audio")]
    public struct TickAudioConfig : IEcsAudioConfig
    {
        [field: SerializeField] public ScriptableEntityTemplate Value { get; set; }

        private sealed class Template : ComponentTemplate<TickAudioConfig>
        {
        }
    }
}