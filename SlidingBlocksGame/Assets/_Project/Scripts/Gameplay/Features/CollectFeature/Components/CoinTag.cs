using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CollectFeature.Components
{
    [Serializable]
    public struct CoinTag : IEcsTagComponent
    {
        private sealed class Template : TagComponentTemplate<CoinTag>
        {
        }
    }
}