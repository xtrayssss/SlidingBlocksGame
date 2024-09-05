using System;
using DCFApixels.DragonECS;
using UnityEngine.Serialization;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct RewardStatus : IEcsComponent
    {
        [FormerlySerializedAs("Collected")] public EcsEntityConnect Locked;
        [FormerlySerializedAs("UnCollected")] public EcsEntityConnect Unlocked;

        private sealed class Template : ComponentTemplate<RewardStatus>
        {
        }
    }
}