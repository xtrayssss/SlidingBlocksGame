using System;
using DCFApixels.DragonECS;
using TMPro;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CoinFeature.IntegrationFeatures.UIFeature.Components
{
    [Serializable]
    [MetaGroup("Coin/UI")]
    public struct CoinWidget : IEcsComponent
    {
        public TextMeshProUGUI AmountText;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.GameProgressFeature.IntegrationFeatures.UIFeature.Components",
            sourceClassName: "CoinWidget/Template", sourceAssembly: "GameProgressFeature.UIFeature.Components")]
        private sealed class Template : ComponentTemplate<CoinWidget>
        {
        }
    }
}