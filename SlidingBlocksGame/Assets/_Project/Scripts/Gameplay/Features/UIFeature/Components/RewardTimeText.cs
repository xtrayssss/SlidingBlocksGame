using System;
using DCFApixels.DragonECS;
using TMPro;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct RewardTimeText : IEcsComponent
    {
        public TextMeshProUGUI Value;

        private sealed class Template : ComponentTemplate<RewardTimeText>
        {
        }
    }
}