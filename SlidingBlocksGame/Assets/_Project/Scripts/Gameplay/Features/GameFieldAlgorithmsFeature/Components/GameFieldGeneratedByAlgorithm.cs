using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Components
{
    [Serializable]
    public struct GameFieldGeneratedByAlgorithm : IEcsComponent
    {
        public entlong Value;
    }
}