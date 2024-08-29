using System;
using _Project.Scripts.Gameplay.Features.ScrollFeature;
using _Project.Scripts.Gameplay.Features.UIFeature.Systems;
using DCFApixels.DragonECS;
using PrimeTween;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct ScrollSnapRef : IEcsComponent
    {
        public EcsEntityConnect Value;
        public Sequence Sequence;

        private sealed class Template : ComponentTemplate<ScrollSnapRef>
        {
        }
    }
}