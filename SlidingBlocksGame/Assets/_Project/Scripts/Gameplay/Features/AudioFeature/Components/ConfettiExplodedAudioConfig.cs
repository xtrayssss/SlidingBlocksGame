using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Components
{
    [Serializable]
    public struct ConfettiExplodedAudioConfig : IEcsAudioConfig
    {
        [field: SerializeField] public ScriptableEntityTemplate Value { get; set; }

        private sealed class Template : ComponentTemplate<ConfettiExplodedAudioConfig>
        {
        }
    }
}