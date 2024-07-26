using System;
using DCFApixels.DragonECS;
using UnityEngine.Serialization;

namespace _Project.Scripts.Gameplay.Features.CooldownFeature.Components
{
    [Serializable]
    public struct Cooldown : IEcsComponent
    {
        [FormerlySerializedAs("Value")] public float Elapsed;
        public float Duration;

        [Serializable]
        public sealed class Wrapper : ComponentTemplate<Cooldown>
        {
        }
    }
}