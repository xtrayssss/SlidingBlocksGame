using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.Components
{
    [Serializable]
    public struct GameTitleTag : IEcsTagComponent
    {
        private sealed class Template : TagComponentTemplate<GameTitleTag>
        {
        }
    }
}