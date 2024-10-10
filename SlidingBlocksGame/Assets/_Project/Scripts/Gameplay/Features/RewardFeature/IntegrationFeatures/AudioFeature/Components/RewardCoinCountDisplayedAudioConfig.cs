using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.RewardFeature.IntegrationFeatures.AudioFeature.Components
{
    [Serializable]
    [MetaGroup("Reward/Audio")]
    public struct RewardCoinCountDisplayedAudioConfig : IEcsComponent
    {
        public ScriptableEntityTemplate Value;

        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.AudioBaseFeature.Components",
            sourceClassName: "RewardCoinCountDisplayedAudioConfig/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<RewardCoinCountDisplayedAudioConfig>
        {
        }
    }
}