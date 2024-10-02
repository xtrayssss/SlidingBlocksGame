using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.Systems
{
    public class TileGenerator6 : MonoBehaviour
    {
        public GameObject tilePrefab; // Префаб плитки
        public int rows = 5; // Количество строк
        public int columns = 10; // Количество столбцов
        public float tileSpacing = 1.5f; // Расстояние между плитками
        public float growthDuration = 2.0f; // Длительность анимации роста
        public float moveDuration = 1.0f; // Длительность анимации перемещения
        public float waveAmplitude = 0.5f; // Амплитуда волны
        public float waveFrequency = 1.0f; // Частота волны

        private List<GameObject> tiles = new List<GameObject>(); // Список всех созданных плиток

        private void Start()
        {
            StartCoroutine(GenerateTiles());
        }

        private IEnumerator GenerateTiles()
        {
            // Создание игрового поля плиток
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    Vector3 startPosition = new Vector3(col * tileSpacing, 0, row * tileSpacing);
                    GameObject tile = Instantiate(tilePrefab, startPosition, Quaternion.identity);
                    tile.transform.localScale = Vector3.zero;
                    tiles.Add(tile); // Добавление плитки в список
                    StartCoroutine(GrowTile(tile));
                    yield return new WaitForSeconds(growthDuration / (rows * columns));
                }
            }
        }

        private IEnumerator GrowTile(GameObject tile)
        {
            Vector3 initialScale = tile.transform.localScale;
            Vector3 targetScale = Vector3.one;
            float elapsedTime = 0;

            // Анимация роста плитки
            while (elapsedTime < growthDuration)
            {
                elapsedTime += Time.deltaTime;
                tile.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / growthDuration);
                yield return null;
            }

            tile.transform.localScale = targetScale;
        }

        // private void Update()
        // {
        //     float time = Time.time * waveFrequency;
        //
        //     for (int i = 0; i < tiles.Count; i++)
        //     {
        //         GameObject tile = tiles[i];
        //         Vector3 position = tile.transform.position;
        //         // Используем синусоидальную функцию для создания эффекта волны
        //         position.y = Mathf.Sin(time + (position.x + position.z) * 0.5f) * waveAmplitude;
        //         tile.transform.position = position;
        //     }
        // }
    }
}