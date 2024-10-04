using System;
using DCFApixels.DragonECS;
using TMPro;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    [MetaGroup("UI")]
    public struct RewardCoinsWidget : IEcsComponent
    {
        public TextMeshProUGUI AmountText;

        private sealed class Template : ComponentTemplate<RewardCoinsWidget>
        {
        }
    }
}