using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Components
{
    [Serializable]
    public struct AudioSourceRef : IEcsComponent
    {
        public AudioSource Value;

        private sealed class Template : ComponentTemplate<AudioSourceRef>
        {
        }
    }
}