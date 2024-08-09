using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CooldownFeature.Components
{
    [Serializable]
    public struct CooldownLockMarker : IEcsTagComponent
    {
        private sealed class Template : TagComponentTemplate<CooldownLockMarker>
        {
        }
    }
}