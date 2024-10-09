using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Templates
{
    [Serializable]
    public class EntityTemplate : ITemplate
    {
// #if UNITY_EDITOR
//         [SerializeField] private string _name;
// #endif

        [SerializeReference] [ComponentTemplateReference]
        private IComponentTemplate[] _components;

        public void Apply(short worldID, int entityID)
        {
            foreach (IComponentTemplate item in _components)
                item.Apply(worldID, entityID);
        }
    }
}