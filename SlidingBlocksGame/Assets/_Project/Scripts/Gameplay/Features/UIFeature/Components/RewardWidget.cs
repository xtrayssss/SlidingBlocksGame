using System;
using DCFApixels.DragonECS;
using PrimeTween;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    [MetaGroup("UI")]
    public struct RewardWidget : IEcsComponent
    {
        public EcsEntityConnect RewardWindowConnect;
        public ClaimRewardWidget ClaimRewardWidget;

        public GameObject Locked;
        public GameObject Unlocked;

        private sealed class Template : ComponentTemplate<RewardWidget>
        {
        }
    }

    [Serializable]
    public struct ClaimRewardWidget
    {
        public Tween Tween;

        public TextMeshProUGUI ClaimRewardText;
        public TextMeshProUGUI RewardTimeText;
    }
}