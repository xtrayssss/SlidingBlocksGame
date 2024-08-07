using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct LevelTag : IEcsTagComponent
    {
        private sealed class Template : TagComponentTemplate<LevelTag>
        {
        }
    }
}