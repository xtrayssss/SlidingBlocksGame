using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.DestroyFeature.Components
{
    [Serializable]
    public struct DestructionStrategyTag : IEcsTagComponent
    {
        private sealed class Template : TagComponentTemplate<DestructionStrategyTag>
        {
        }
    }
}