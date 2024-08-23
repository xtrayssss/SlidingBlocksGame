using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct AnimalPurchases : IEcsComponent
    {
        public EcsGroup Entities;
        public EcsEntityConnect[] Prefabs;

        private sealed class Template : ComponentTemplate<AnimalPurchases>
        {
        }
    }
}