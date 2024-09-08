using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components
{
    [Serializable]
    public struct MetaGameUIHiddenMarker : IEcsTagComponent
    {
        private sealed class Template : TagComponentTemplate<MetaGameUIHiddenMarker>
        {
        }
    }
}