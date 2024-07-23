using System;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components
{
    [Serializable]
    public struct GameLevels : IEcsComponent
    {
        public Level[] Levels;

        [Serializable]
        public struct Level
        {
            public Unit[] Units;
            
            [Serializable]
            public struct Unit
            {
                public float2 Position;
                public GameObject Prefab;
            }
        }

        private sealed class Template : ComponentTemplate<GameLevels>
        {
        }
    }
}