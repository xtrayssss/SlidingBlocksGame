using System;
using DCFApixels.DragonECS;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.UIFeature.Components
{
    [Serializable]
    [MetaGroup("Reward/UI")]
    public struct RewardWidget : IEcsComponent
    {
        public EcsEntityConnect RewardWindowConnect;
        public ClaimRewardWidget ClaimRewardWidget;

        public GameObject Locked;
        public GameObject Unlocked;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components",
            sourceClassName: "RewardWidget/Template", sourceAssembly: "Assembly-CSharp")]
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