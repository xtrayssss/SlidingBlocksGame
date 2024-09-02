using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components
{
    [Serializable]
    public struct CoinUIConnect : IEcsComponent
    {
        public EcsEntityConnect Value;

        private sealed class Template : ComponentTemplate<CoinUIConnect>
        {
        }
    }
}