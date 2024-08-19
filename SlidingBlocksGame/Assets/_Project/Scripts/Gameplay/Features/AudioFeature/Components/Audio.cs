using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Components
{
    [Serializable]
    public struct Audio : IEcsComponent
    {
        public AudioClip Value;
    
        private sealed class Template : ComponentTemplate<Audio>
        {
        }
    }
}