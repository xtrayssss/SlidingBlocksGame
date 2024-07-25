using System;
using System.ComponentModel.Design.Serialization;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

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

        public Unit[] Units;

        [Serializable]
        public struct Unit
        {
            public float3 Position;
            public EcsEntityConnect Prefab;
            public float3 Rotation;
        }

        public class Template : ComponentTemplate<GameField>
        {
        }
    }
}