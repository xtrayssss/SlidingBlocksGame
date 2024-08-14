using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct BestTag : IEcsTagComponent
    {
        private sealed class Template : TagComponentTemplate<BestTag>
        {
        }
    }
}