using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.RewardFeature.Components
{
    [Serializable]
    [MetaGroup("Reward")]
    public struct Reward : IEcsComponent
    {
        public long CollectionTime;
        public long Interval;
        public int ClaimedCount;

        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.RewardFeature.Components",
            sourceClassName: "Reward/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<Reward>
        {
        }
    }
}