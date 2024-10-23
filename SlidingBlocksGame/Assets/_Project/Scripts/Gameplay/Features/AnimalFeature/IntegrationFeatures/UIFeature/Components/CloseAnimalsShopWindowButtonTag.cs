using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.UIFeature.Components
{
    [Serializable]
    [MetaGroup("Animal/UI")]
    public struct CloseAnimalsShopWindowButtonTag : IEcsTagComponent
    {
        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components",
            sourceClassName: "CloseAnimalsShopWindowButtonTag/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : TagComponentTemplate<CloseAnimalsShopWindowButtonTag>
        {
        }
    }

    [Serializable]
    public struct CloseAnimalPurchaseWindowButtonTag : IEcsTagComponent
    {
        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components",
            sourceClassName: "CloseAnimalPurchaseWindowButtonTag/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : TagComponentTemplate<CloseAnimalPurchaseWindowButtonTag>
        {
        }
    }
}