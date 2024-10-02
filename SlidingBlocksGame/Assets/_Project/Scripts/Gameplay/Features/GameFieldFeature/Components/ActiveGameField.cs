using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameFieldFeature.Components
{
    [Serializable]
    public struct ActiveGameField : IEcsComponent
    {
        public entlong Value;
    }
}