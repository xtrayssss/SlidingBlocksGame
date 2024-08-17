using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct AnimalsShopWindowTag : IEcsTagComponent
    {
        private sealed class Template : TagComponentTemplate<AnimalsShopWindowTag>
        {
        }
    }
}