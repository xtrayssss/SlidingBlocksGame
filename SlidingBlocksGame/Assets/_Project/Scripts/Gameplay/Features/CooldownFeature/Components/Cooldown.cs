using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CooldownFeature.Components
{
    [Serializable]
    public struct Cooldown : IEcsComponent
    {
        public float Value;
        public float Duration;

        [Serializable]
        public sealed class Wrapper : ComponentTemplate<Cooldown>
        {
        }
    }
}