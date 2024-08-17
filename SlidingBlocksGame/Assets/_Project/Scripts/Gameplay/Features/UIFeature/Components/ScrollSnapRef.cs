using System;
using _Project.Scripts.Gameplay.Features.ScrollFeature;
using _Project.Scripts.Gameplay.Features.UIFeature.Systems;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct ScrollSnapRef : IEcsComponent
    {
        public ScrollSnap Value;

        private sealed class Template : ComponentTemplate<ScrollSnapRef>
        {
        }
    }
}