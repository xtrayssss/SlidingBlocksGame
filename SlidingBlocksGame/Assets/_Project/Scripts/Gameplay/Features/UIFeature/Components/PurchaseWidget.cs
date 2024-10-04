using System;
using DCFApixels.DragonECS;
using UnityEngine.UI;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    [MetaGroup("UI")]
    public struct PurchaseWidget : IEcsComponent
    {
        public PurchaseStatusWidget PurchaseStatusWidget;
        public RawImage Icon;
        
        private sealed class Template : ComponentTemplate<PurchaseWidget>
        {
        }
    }
}