using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components
{
    [Serializable]
    public struct GameField : IEcsComponent
    {
        public GameObject TilePrefab;
        public Vector3 OriginPosition;
        public int Size;
        public float Offset;
        public int CellSize;
        public int CenterSize;
        public int EdgeSize;

        public class Template : ComponentTemplate<GameField>
        {
        }
    }
}