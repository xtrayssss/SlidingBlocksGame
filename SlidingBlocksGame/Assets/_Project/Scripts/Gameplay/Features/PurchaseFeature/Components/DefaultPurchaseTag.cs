using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.PurchaseFeature.Components
{
    [Serializable]
    [MetaGroup("Purchase")]
    public struct DefaultPurchaseTag : IEcsTagComponent
    {
        private sealed class Template : TagComponentTemplate<DefaultPurchaseTag>
        {
        }
    }
}