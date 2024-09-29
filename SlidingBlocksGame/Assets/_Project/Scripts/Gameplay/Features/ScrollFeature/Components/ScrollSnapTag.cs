using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.ScrollFeature.Components
{
    [Serializable]
    public struct ScrollSnapTag : IEcsTagComponent
    {
        private sealed class Template : TagComponentTemplate<ScrollSnapTag>
        {
        }
    }
}