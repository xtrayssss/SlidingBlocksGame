using System;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components
{
    [Serializable]
    [MetaGroup("UI")]
    public struct GameTitleWidget : IEcsComponent
    {
        public Tween WobbleTween;
        
        private sealed class Template : ComponentTemplate<GameTitleWidget>
        {
        }
    }
}