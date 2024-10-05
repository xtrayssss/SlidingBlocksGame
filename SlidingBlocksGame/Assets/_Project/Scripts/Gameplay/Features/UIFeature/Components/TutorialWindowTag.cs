using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    [MetaGroup("UI")]
    public struct TutorialWindowTag : IEcsTagComponent
    {
        private sealed class Template : TagComponentTemplate<TutorialWindowTag>
        {
        }
    }
}