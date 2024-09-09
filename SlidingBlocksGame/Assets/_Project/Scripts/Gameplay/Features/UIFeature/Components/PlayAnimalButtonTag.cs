using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct PlayAnimalButtonTag : IEcsTagComponent
    {
        private sealed class Template : TagComponentTemplate<PlayAnimalButtonTag>
        {
        }
    }
}