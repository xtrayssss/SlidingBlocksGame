using System.Collections;
using UnityEngine;

namespace _Project.Scripts
{
    public class TileSpawner : MonoBehaviour
    {
        public GameObject tilePrefab; // Префаб плитки
        public TileGrid tileGrid; // Ссылка на сетку

        public float flyDuration = 1.0f; // Время полета плитки
        public float spawnInterval = 0.5f; // Интервал появления плиток

    
        void Start()
        {
            StartCoroutine(SpawnTiles());
        }

        IEnumerator SpawnTiles()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawnInterval);

                // Выбираем случайную ячейку
                int randomX = Random.Range(0, tileGrid.gridWidth);
                int randomY = Random.Range(0, tileGrid.gridHeight);
                Transform targetCell = tileGrid.gridCells[randomX, randomY];

                // Создаем плитку вне экрана, выбираем случайную сторону
                Vector3 spawnPosition = GetRandomSpawnPosition();
                GameObject newTile = Instantiate(tilePrefab, spawnPosition, Quaternion.identity);

                // Анимируем прилет плитки к цели
                StartCoroutine(FlyTile(newTile.transform, targetCell.position));
            }
        }

        Vector3 GetRandomSpawnPosition()
        {
            // Выбираем случайную сторону экрана: 0 - сверху, 1 - снизу, 2 - слева, 3 - справа
            int side = Random.Range(0, 4);
            Vector3 spawnPosition = Vector3.zero;
            Camera cam = Camera.main;

            switch (side)
            {
                case 0: // сверху
                    spawnPosition = new Vector3(Random.Range(0, Screen.width), Screen.height, 0);
                    break;
                case 1: // снизу
                    spawnPosition = new Vector3(Random.Range(0, Screen.width), 0, 0);
                    break;
                case 2: // слева
                    spawnPosition = new Vector3(0, Random.Range(0, Screen.height), 0);
                    break;
                case 3: // справа
                    spawnPosition = new Vector3(Screen.width, Random.Range(0, Screen.height), 0);
                    break;
            }

            // Преобразуем координаты экрана в мировые координаты
            return cam.ScreenToWorldPoint(new Vector3(spawnPosition.x, spawnPosition.y, cam.nearClipPlane));
        }

        IEnumerator FlyTile(Transform tile, Vector3 targetPosition)
        {
            Vector3 startPosition = tile.position;
            float elapsedTime = 0f;

            while (elapsedTime < flyDuration)
            {
                tile.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / flyDuration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            tile.position = targetPosition; // Устанавливаем финальную позицию
        }
    }
}