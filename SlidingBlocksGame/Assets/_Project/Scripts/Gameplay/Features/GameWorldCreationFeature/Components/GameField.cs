using System;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components
{
    [Serializable]
    public struct GameField : IEcsComponent
    {
        [FormerlySerializedAs("blockPrefab")] public GameObject BlockPrefab;

        [FormerlySerializedAs("originPosition")]
        public Vector3 OriginPosition;

        [FormerlySerializedAs("size")] public int Size;
        [FormerlySerializedAs("offset")] public float Offset;
        [FormerlySerializedAs("cellSize")] public int CellSize;
        public int CenterSize;
        public int EdgeSize;

        public class Template : ComponentTemplate<GameField>
        {
        }
    }
}