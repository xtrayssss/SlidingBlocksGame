using System;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts.Gameplay.Features.CooldownFeature.Components
{
    [Serializable]
    public struct CooldownInterval : IEcsComponent
    {
        public float Elapsed;
        public float Interval;

        private sealed class Template : ComponentTemplate<CooldownInterval>
        {
        }
    }
}