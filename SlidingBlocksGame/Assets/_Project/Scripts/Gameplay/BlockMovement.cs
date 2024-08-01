using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BlockMovement : MonoBehaviour
{
    public List<Vector2Int> startPositions;

    public List<Vector2Int> obstacles;

    public Vector2Int destination;

    [Button]
    void Start()
    {
        // Пример стартовых позиций блоков
        // Центральные ячейки

        int centerX = 2 + 2 / 2;
        int centerZ = 2 + 2 / 2;

        int centerMin = 2;
        int centerMax = 4;

        foreach (var startPosition in startPositions)
        {
            var center = 0;

            var target = destination;

            if (startPosition.x < 2)
            {
                target = new Vector2Int(Mathf.Abs(1 - startPosition.x - target.x), startPosition.y);
            }
            else if (startPosition.x >= 2 + 2)
            {
                target = new Vector2Int(Mathf.Abs(4 - startPosition.x) + target.x, startPosition.y);
            }

            if (startPosition.y >= 2 + 2)
            {
                target = new Vector2Int(startPosition.x, Mathf.Abs(4 - startPosition.y) + target.y);
            }
            else if (startPosition.y < 2)
            {
                target = new Vector2Int(startPosition.x, Mathf.Abs(1 - startPosition.y - target.y));
            }
        }
    }

    List<Vector2Int> GetTargetPositions(List<Vector2Int> startPositions, List<Vector2Int> centralPositions,
        List<Vector2Int> allBlocks)
    {
        List<Vector2Int> targetPositions = new List<Vector2Int>();

        foreach (var startPos in startPositions)
        {
            Vector2Int targetPos = GetTargetPositionForBlock(startPos, centralPositions, allBlocks);
            targetPositions.Add(targetPos);
        }

        return targetPositions;
    }

    Vector2Int GetTargetPositionForBlock(Vector2Int startPos, List<Vector2Int> centralPositions,
        List<Vector2Int> allBlocks)
    {
        foreach (var centralPos in centralPositions)
        {
            bool pathBlocked = false;
            if (startPos.x == centralPos.x)
            {
                int minY = Mathf.Min(startPos.y, centralPos.y);
                int maxY = Mathf.Max(startPos.y, centralPos.y);

                for (int y = minY + 1; y < maxY; y++)
                {
                    if (allBlocks.Contains(new Vector2Int(startPos.x, y)))
                    {
                        pathBlocked = true;
                        return new Vector2Int(startPos.x, y - 1);
                    }
                }
            }
            else if (startPos.y == centralPos.y)
            {
                int minX = Mathf.Min(startPos.x, centralPos.x);
                int maxX = Mathf.Max(startPos.x, centralPos.x);

                for (int x = minX + 1; x < maxX; x++)
                {
                    if (allBlocks.Contains(new Vector2Int(x, startPos.y)))
                    {
                        pathBlocked = true;
                        return new Vector2Int(x - 1, startPos.y);
                    }
                }
            }

            if (!pathBlocked)
            {
                return centralPos;
            }
        }

        return startPos; // Если не нашли путь, возвращаем стартовую позицию
    }
}