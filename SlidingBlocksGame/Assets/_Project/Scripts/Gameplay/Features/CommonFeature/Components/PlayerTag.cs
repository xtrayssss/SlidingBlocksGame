using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct PlayerTag : IEcsTagComponent
    {
        private sealed class Template : TagComponentTemplate<PlayerTag>
        {
        }
    }
}