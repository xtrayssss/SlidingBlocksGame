using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CooldownFeature.Components
{
    [Serializable]
    public struct Cooldown : IEcsComponent
    {
        public float Elapsed;
        public float Duration;

        public sealed class Template : ComponentTemplate<Cooldown>
        {
        }
    }
}