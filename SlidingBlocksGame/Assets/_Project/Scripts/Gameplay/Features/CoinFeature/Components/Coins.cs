using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CoinFeature.Components
{
    [Serializable]
    [MetaGroup("Coin")]
    public struct Coins : IEcsComponent
    {
        public int Value;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.GameProgressFeature.Components",
            sourceClassName: "Coins/Template", sourceAssembly: "GameProgressFeature.Components")]
        private sealed class Template : ComponentTemplate<Coins>
        {
        }
    }
}