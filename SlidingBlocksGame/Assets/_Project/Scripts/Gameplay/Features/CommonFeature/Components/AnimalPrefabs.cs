using System;
using System.Collections.Generic;
using System.Linq;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct AnimalPrefabs : IEcsComponent
    {
        [SerializeField] private AnimalEntityConnect[] _animals;
        
        public Dictionary<uint, AnimalEntityConnect> Animals;

        public int value;

        private sealed class Template : ComponentTemplate<AnimalPrefabs>
        {
            public override void Apply(short worldID, int entityID)
            {
                component.Animals = new Dictionary<uint, AnimalEntityConnect>();

                IEnumerable<int> indices = Enumerable.Range(0, component._animals.Length);
                
                foreach (uint index in indices) 
                    component.Animals.Add(index, component._animals[index]);
                
                base.Apply(worldID, entityID);
            }
        }
    }
}