using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct SettingsPopupTag : IEcsTagComponent
    {
        private sealed class Template : TagComponentTemplate<SettingsPopupTag>
        {
        }
    }
}