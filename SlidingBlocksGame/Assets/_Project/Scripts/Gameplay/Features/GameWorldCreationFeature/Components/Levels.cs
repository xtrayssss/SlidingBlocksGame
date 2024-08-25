using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components
{
    [Serializable]
    public struct Levels : IEcsComponent
    {
        public int LevelIndex;
        public int PackIndex;
        
        public LevelsPack[] Value;

        [Serializable]
        public struct LevelsPack
        {
            public ScriptableEntityTemplate[] Levels;
        }

        private sealed class Template : ComponentTemplate<Levels>
        {
        }
    }
}