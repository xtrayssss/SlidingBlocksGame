using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CoinFeature.Components
{
    [Serializable]
    [MetaGroup("Coin")]
    public struct CoinPrefab : IEcsComponent
    {
        public EcsEntityConnect Prefab;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.GameProgressFeature.Components",
            sourceClassName: "CoinPrefab/Template", sourceAssembly: "GameProgressFeature.Components")]
        private sealed class Template : ComponentTemplate<CoinPrefab>
        {
        }
    }
}