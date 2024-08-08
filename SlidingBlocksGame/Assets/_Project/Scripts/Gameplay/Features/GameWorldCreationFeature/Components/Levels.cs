using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components
{
    [Serializable]
    public struct Levels : IEcsComponent
    {
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