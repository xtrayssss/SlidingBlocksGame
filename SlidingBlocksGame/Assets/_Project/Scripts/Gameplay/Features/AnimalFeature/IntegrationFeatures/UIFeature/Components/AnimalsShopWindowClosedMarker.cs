using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.UIFeature.Components
{
    [Serializable]
    [MetaGroup("Animal/UI")]
    public struct AnimalsShopWindowClosedMarker : IEcsTagComponent 
    {
        private sealed class Template : TagComponentTemplate<AnimalsShopWindowClosedMarker>
        {
        }
    }
}