using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Utils
{
    public static class GridUtils
    {
        public static readonly int2 UP = new int2(0, 1);
        public static readonly int2 DOWN = new int2(0, -1);
        public static readonly int2 LEFT = new int2(-1, 0);
        public static readonly int2 RIGHT = new int2(1, 0);

        public static bool IsWithinCenter(float2 position, in GameField gameField)
        {
            return position.x >= gameField.EdgeSize &&
                   position.x < gameField.EdgeSize + gameField.CenterSize &&
                   position.y >= gameField.EdgeSize &&
                   position.y < gameField.EdgeSize + gameField.CenterSize;
        }

        public static int2 GetCenter(in GameField gameField) =>
            new int2(gameField.EdgeSize, gameField.CenterSize + gameField.EdgeSize - 1);

        public static int2 GetInvertedSide(float2 position, in GameField gameField)
        {
            if (position.x < gameField.EdgeSize)
                return RIGHT;
            if (position.x >= gameField.EdgeSize + gameField.CenterSize)
                return LEFT;
            if (position.y < gameField.EdgeSize)
                return UP;
            if (position.y >= gameField.EdgeSize + gameField.CenterSize)
                return DOWN;

            return default;
        }

        public static int2 GetCellPosition(float3 worldPosition, in GameField gameField)
        {
            int x = (int)math.floor(
                (worldPosition.x - gameField.OriginPosition.x + gameField.CellSize * 0.5f + gameField.Offset * 0.5f) /
                (gameField.CellSize + gameField.Offset));

            int z = (int)math.floor(
                (worldPosition.z - gameField.OriginPosition.z + gameField.CellSize * 0.5f + gameField.Offset * 0.5f) /
                (gameField.CellSize + gameField.Offset));

            return new int2(x, z);
        }

        public static float3 GetWorldPosition(int2 coordinates, in GameField gameField) =>
            new float3(coordinates.x * (gameField.CellSize + gameField.Offset) + gameField.OriginPosition.x, 0,
                coordinates.y * (gameField.CellSize + gameField.Offset) + gameField.OriginPosition.z);

        public static void SetCell(int2 position, ref GameField gameField)
        {
            int2 dimensions = position.yx - gameField.EdgeSize;

            int bitPosition = dimensions.x * gameField.EdgeSize + dimensions.y;

            gameField.Center |= (short)(1 << bitPosition);

            DebugGameField(in gameField);

            void DebugGameField(in GameField gameField)
            {
                string fieldRepresentation = "GameField:\n";

                for (int row = 0; row < gameField.EdgeSize; row++)
                {
                    for (int col = 0; col < gameField.EdgeSize; col++)
                    {
                        bool isOccupied = GetCell(new int2(row, col), in gameField);
                        fieldRepresentation += isOccupied ? "1 " : "0 ";
                    }

                    fieldRepresentation += "\n";
                }

                Debug.Log(fieldRepresentation);
            }
        }

        public static bool GetCell(int2 dimensions, in GameField gameField)
        {
            int bitPosition = dimensions.x * gameField.EdgeSize + dimensions.y;
            return (gameField.Center & (1 << bitPosition)) != 0;
        }

        public static (int2 obstacle, bool success) GetNearestCentralObstacle(int2 position, int2 side,
            in GameField gameField)
        {
            int2 dimensions = new int2();

            if (side.x == 1)
            {
                dimensions.x = position.y - gameField.EdgeSize;
                dimensions.y = 0;
            }
            else if (side.x == -1)
            {
                dimensions.x = position.y - gameField.EdgeSize;
                dimensions.y = gameField.EdgeSize - 1;
            }
            else if (side.y == 1)
            {
                dimensions.x = 0;
                dimensions.y = position.x - gameField.EdgeSize;
            }
            else if (side.y == -1)
            {
                dimensions.x = gameField.EdgeSize - 1;
                dimensions.y = position.x - gameField.EdgeSize;
            }

            side = side.yx;

            int step = 0;

            do
            {
                int2 tempDimensions = dimensions + side * step;

                Debug.Log(tempDimensions);

                int bitPosition = tempDimensions.x * gameField.EdgeSize + tempDimensions.y;

                Debug.Log(bitPosition);

                if ((gameField.Center & (1 << bitPosition)) != 0)
                {
                    Debug.Log("HAS OBSTACLE " + bitPosition);

                    return (tempDimensions.yx, true);
                }
            } while (step++ != gameField.EdgeSize - 1);

            return default;
        }

        public static void Catch<TRequest>(EcsWorld world, int target) where TRequest : struct, IEcsTagComponent
        {
            int @event = world.NewEntity();

            world.GetPool<TRequest>().Add(@event);
            world.GetPool<TargetEntity>().Add(@event).Value = target.ToEntityLong(world);
        }
    }
}