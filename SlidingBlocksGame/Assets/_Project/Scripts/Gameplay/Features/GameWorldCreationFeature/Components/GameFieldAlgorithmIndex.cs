using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components
{
    [Serializable]
    public struct GameFieldAlgorithmIndex : IEcsComponent
    {
        public uint Value;
    }
}