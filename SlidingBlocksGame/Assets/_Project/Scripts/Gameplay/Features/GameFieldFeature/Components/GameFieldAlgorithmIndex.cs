using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameFieldFeature.Components
{
    [Serializable]
    [MetaGroup("GameField")]
    public struct GameFieldAlgorithmIndex : IEcsComponent
    {
        public uint Value;
    }
}