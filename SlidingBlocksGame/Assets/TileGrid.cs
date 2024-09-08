using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public int gridWidth = 5; // Ширина сетки
    public int gridHeight = 5; // Высота сетки
    public float tileSize = 1.1f; // Размер ячейки

    public Transform[,] gridCells; // Массив для ячеек

    void Start()
    {
        gridCells = new Transform[gridWidth, gridHeight];
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Вычисляем позицию каждой ячейки
                Vector3 cellPosition = new Vector3(x * tileSize, y * tileSize, 0);
                // Создаем пустую ячейку для обозначения позиции
                GameObject cell = new GameObject($"Cell {x},{y}");
                cell.transform.position = cellPosition;
                cell.transform.parent = this.transform;
                gridCells[x, y] = cell.transform; // Сохраняем ячейки
            }
        }
    }
}