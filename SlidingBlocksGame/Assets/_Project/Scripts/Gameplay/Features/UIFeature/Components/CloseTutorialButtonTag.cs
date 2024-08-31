using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct CloseTutorialButtonTag : IEcsTagComponent
    {
        private sealed class Template : TagComponentTemplate<CloseTutorialButtonTag>
        {
        }
    }
}