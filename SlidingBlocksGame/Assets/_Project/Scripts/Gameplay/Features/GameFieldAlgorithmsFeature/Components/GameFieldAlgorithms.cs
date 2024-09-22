using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Components
{
    [Serializable]
    [MetaGroup("GameField")]
    public struct GameFieldAlgorithms : IEcsComponent
    {
        public EntityTemplate[] Value;

        private sealed class Template : ComponentTemplate<GameFieldAlgorithms>
        {
        }
    }
}