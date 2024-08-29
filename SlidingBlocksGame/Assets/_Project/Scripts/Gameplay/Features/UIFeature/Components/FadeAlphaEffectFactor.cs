using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct FadeAlphaEffectFactor : IEcsComponent
    {
        public float Value;

        private sealed class Template : ComponentTemplate<FadeAlphaEffectFactor>
        {
        }
    }
}