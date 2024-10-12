using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components
{
    [Serializable]
    [MetaGroup("ScrollSnap")]
    public struct FadeEffect : IEcsComponent
    {
        public float FadeAlpha;

        public sealed class Template : ComponentTemplate<FadeEffect>
        {
        }
    }
}