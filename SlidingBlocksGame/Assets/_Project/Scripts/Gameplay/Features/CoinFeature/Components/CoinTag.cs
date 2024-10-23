using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CoinFeature.Components
{
    [Serializable]
    [MetaGroup("Coin")]
    public struct CoinTag : IEcsTagComponent
    {
        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.GameProgressFeature.Components",
            sourceClassName: "CoinTag/Template", sourceAssembly: "GameProgressFeature.Components")]
        private sealed class Template : TagComponentTemplate<CoinTag>
        {
        }
    }
}