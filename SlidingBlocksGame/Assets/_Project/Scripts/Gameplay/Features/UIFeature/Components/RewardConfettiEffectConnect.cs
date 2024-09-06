using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct RewardConfettiEffectConnect : IEcsComponent
    {
        public EcsEntityConnect Value;

        private sealed class Template : ComponentTemplate<RewardConfettiEffectConnect>
        {
        }
    }
}