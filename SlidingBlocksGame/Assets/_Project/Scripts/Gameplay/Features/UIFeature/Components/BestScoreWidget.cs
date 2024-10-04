using System;
using DCFApixels.DragonECS;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    [MetaGroup("UI")]
    public struct BestScoreWidget : IEcsComponent
    {
        public TextMeshProUGUI AmountText;
        
        private sealed class Template : ComponentTemplate<BestScoreWidget>
        {
        }
    }
}