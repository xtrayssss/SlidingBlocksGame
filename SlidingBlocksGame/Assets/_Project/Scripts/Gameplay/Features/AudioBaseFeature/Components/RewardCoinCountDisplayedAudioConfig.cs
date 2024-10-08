using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.AudioBaseFeature.Components
{
    [Serializable]
    [MetaGroup("Audio")]
    public struct RewardCoinCountDisplayedAudioConfig : IEcsComponent
    {
        public ScriptableEntityTemplate Value;

        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.AudioBaseFeature.Components",
            sourceClassName: "CoinAddedToTextAudioConfig/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<RewardCoinCountDisplayedAudioConfig>
        {
        }
    }
}