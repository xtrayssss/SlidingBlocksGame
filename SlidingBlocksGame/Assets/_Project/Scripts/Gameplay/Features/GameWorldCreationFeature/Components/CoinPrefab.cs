using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components
{
    [Serializable]
    public struct CoinPrefab : IEcsComponent
    {
        public EcsEntityConnect Prefab;

        private sealed class Template : ComponentTemplate<CoinPrefab>
        {
        }
    }
}