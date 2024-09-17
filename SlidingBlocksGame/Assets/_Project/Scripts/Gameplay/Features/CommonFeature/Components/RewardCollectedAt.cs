using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct RewardCollectedAt : IEcsComponent
    {
        public long Value;

        private sealed class Template : ComponentTemplate<RewardCollectedAt>
        {
        }
    }
}