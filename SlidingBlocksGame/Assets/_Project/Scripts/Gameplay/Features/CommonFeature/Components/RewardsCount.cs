using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct RewardsCount : IEcsComponent
    {
        public int Value;

        private sealed class Template : ComponentTemplate<RewardsCount>
        {
        }
    }
}