using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay
{
    [Serializable]
    public class TemporaryEntityTemplate : ITemplate
    {
        [SerializeReference]
        private ComponentTemplateBase[] _components;

        public void Apply(short worldID, int entityID)
        {
            foreach (var item in _components)
            {
                item.Apply(worldID, entityID);
            }
        }
    }
}