using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components
{
    [Serializable]
    public struct GameTag : IEcsTagComponent
    {
        [Serializable]
        public sealed class Template : TagComponentTemplate<GameTag>
        {
        }
    }
}