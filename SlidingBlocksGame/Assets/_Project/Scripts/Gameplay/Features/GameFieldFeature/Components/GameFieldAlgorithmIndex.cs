using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameFieldFeature.Components
{
    [Serializable]
    [MetaGroup("GameField")]
    public struct GameFieldAlgorithmIndex : IEcsComponent
    {
        public uint Value;
    }
}