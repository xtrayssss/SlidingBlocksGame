using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.EasingFeature.Components
{
    [Serializable]
    public struct EasingSpeed : IEcsComponent
    {
        public float Value;

        [Serializable]
        public sealed class Wrapper : ComponentTemplate<EasingSpeed>
        {
        }
    }
}