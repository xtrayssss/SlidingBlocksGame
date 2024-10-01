using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components
{
    [Serializable]
    public struct ScrollPosition : IEcsComponent
    {
        public float Value;
    }
}