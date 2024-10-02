using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameFieldFeature.Components
{
    [Serializable]
    public struct GameFieldGeneratedByAlgorithm : IEcsComponent
    {
        public entlong Value;
    }
}