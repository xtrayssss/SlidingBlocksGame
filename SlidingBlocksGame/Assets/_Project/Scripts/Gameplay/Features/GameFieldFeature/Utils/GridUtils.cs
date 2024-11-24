using Unity.Mathematics;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameFieldFeature.Utils
{
    public static class GridUtils
    {
        public struct Grid
        {
            public float CellSize;
            public float Offset;
            public float3 OriginPosition;
        }

        private static readonly int2 UP = new int2(0, 1);
        private static readonly int2 DOWN = new int2(0, -1);
        private static readonly int2 LEFT = new int2(-1, 0);
        private static readonly int2 RIGHT = new int2(1, 0);

        public static bool IsWithinCenter(int2 position, int edgeSize, int centerSize)
        {
            return position.x >= edgeSize &&
                   position.x < edgeSize + centerSize &&
                   position.y >= edgeSize &&
                   position.y < edgeSize + centerSize;
        }

        public static int2 GetCenter(int edgeSize, int centerSize) =>
            new int2(edgeSize, centerSize + edgeSize - 1);

        public static int2 GetInvertedSide(float2 position, int edgeSize, int centerSize)
        {
            if (position.x < edgeSize)
                return RIGHT;
            if (position.x >= edgeSize + centerSize)
                return LEFT;
            if (position.y < edgeSize)
                return UP;
            if (position.y >= edgeSize + centerSize)
                return DOWN;

            return default;
        }

        public static int2 GetCellPosition(float3 worldPosition, Grid grid)
        {
            int x = (int)math.floor(
                (worldPosition.x - grid.OriginPosition.x + grid.CellSize * 0.5f + grid.Offset * 0.5f) /
                (grid.CellSize + grid.Offset));

            int z = (int)math.floor(
                (worldPosition.z - grid.OriginPosition.z + grid.CellSize * 0.5f + grid.Offset * 0.5f) /
                (grid.CellSize + grid.Offset));

            return new int2(x, z);
        }

        public static float3 GetWorldPosition(int2 coordinates, in Grid grid) =>
            new float3(coordinates.x * (grid.CellSize + grid.Offset) + grid.OriginPosition.x, 0,
                coordinates.y * (grid.CellSize + grid.Offset) + grid.OriginPosition.z);

        public static void SetCell(int2 position, int edgeSize, ref short grid)
        {
            int2 dimensions = position.yx - edgeSize;

            int bitPosition = dimensions.x * edgeSize + dimensions.y;

            grid |= (short)(1 << bitPosition);

            DebugGameField(in grid);

            void DebugGameField(in short grid)
            {
                string fieldRepresentation = "GameField:\n";

                for (int row = 0; row < edgeSize; row++)
                {
                    for (int col = 0; col < edgeSize; col++)
                    {
                        bool isOccupied = GetCell(new int2(row, col), edgeSize, in grid);
                        fieldRepresentation += isOccupied ? "1 " : "0 ";
                    }

                    fieldRepresentation += "\n";
                }

#if DEBUG
                Debug.Log(fieldRepresentation);
#endif
            }
        }

        public static bool GetCell(int2 dimensions, int edgeSize, in short gameField)
        {
            int bitPosition = dimensions.x * edgeSize + dimensions.y;
            return (gameField & (1 << bitPosition)) != 0;
        }

        public static bool IsWithinGrid(int2 position, int size)
        {
            return position.x >= 0 && position.x < size &&
                   position.y >= 0 && position.y < size;
        }

        public static bool IsInCross(int2 position, int edgeSize, int centerSize)
        {
            return (position.x >= edgeSize && position.x < edgeSize + centerSize) ||
                   (position.y >= edgeSize && position.y < edgeSize + centerSize);
        }
    }
}