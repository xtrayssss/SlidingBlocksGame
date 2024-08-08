using System;
using DCFApixels.DragonECS;
using DCFApixels.DragonECS.Unity.Internal;
using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components
{
    [Serializable]
    public struct Levels : IEcsComponent
    {
        [SerializeReference]
        public TemporaryEntityTemplate[] Templates;
        
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