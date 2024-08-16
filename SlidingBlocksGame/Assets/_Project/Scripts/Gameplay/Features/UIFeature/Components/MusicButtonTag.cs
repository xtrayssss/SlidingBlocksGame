using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct MusicButtonTag : IEcsTagComponent
    {
        private sealed class Template : TagComponentTemplate<MusicButtonTag>
        {
        }
    }
}