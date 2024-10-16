using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components
{
    [Serializable]
    public struct OriginalAnchoredPosition : IEcsComponent
    {
        public Vector2 Value;
    }
}