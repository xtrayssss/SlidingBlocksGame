using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct RectTransformRef : IEcsComponent
    {
        public RectTransform Value;

        private sealed class Template : ComponentTemplate<RectTransformRef>
        {
        }
    }
}