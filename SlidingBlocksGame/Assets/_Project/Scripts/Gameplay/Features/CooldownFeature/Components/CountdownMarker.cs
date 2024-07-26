using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CooldownFeature.Components
{
    [Serializable]
    public struct CountdownMarker : IEcsTagComponent
    {
        [Serializable]
        public sealed class Template : TagComponentTemplate<CountdownMarker>
        {
        }
    }
}