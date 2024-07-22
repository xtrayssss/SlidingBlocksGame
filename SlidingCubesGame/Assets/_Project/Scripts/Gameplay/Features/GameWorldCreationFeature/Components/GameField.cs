using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components
{
    [Serializable]
    public struct GameField : IEcsComponent
    {
        public GameObject blockPrefab;
        public Vector3 originPosition;
        public int size;
        public float offset;
        public int cellSize;

        public class Template : ComponentTemplate<GameField>
        {
        }
    }
}