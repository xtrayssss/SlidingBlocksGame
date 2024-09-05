using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct RewardInterval : IEcsComponent
    {
        public long Value;

        private sealed class Template : ComponentTemplate<RewardInterval>
        {
        }
    }
}