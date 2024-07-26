using System;
using DCFApixels.DragonECS;
using UnityEditor;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components
{
    [Serializable]
    public struct Levels : IEcsComponent
    {
        public Level[] Value;

        [Serializable]
        public struct Level : ITemplate
        {
            public SceneAsset Scene;
            public GameField Field;
            
            public void Apply(short worldID, int entityID)
            {
                GameField.Template template = new GameField.Template();
                template.Apply(worldID, entityID);
            }
        }

        private class Template : ComponentTemplate<Levels>
        {
        }
    }
}