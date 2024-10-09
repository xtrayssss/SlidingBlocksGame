using System;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components
{
    [Serializable]
    [MetaGroup("ScrollSnap")]
    public struct ScrollElement : IEcsComponent
    {
        public Graphic Graphic;
        public float Position;
        public RectTransform RectTransform;

        private sealed class Template : ComponentTemplate<ScrollElement>
        {
        }
    }
}