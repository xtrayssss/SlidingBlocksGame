using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct LevelCounter : IEcsComponent
    {
        public int Value;
        public int Pack;
    }
}