using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class DominantColorFinder : MonoBehaviour
{
    public MeshRenderer targetRenderer;
    public MeshFilter meshFilter;
    
    public SpriteRenderer a;
    public SpriteRenderer b;
    [SerializeField] private ParticleSystem particleSystem2;

    [Button]
    void Start()
    {
        // // Получаем текстуру из материала
        // Texture2D texture = (Texture2D)targetRenderer.material.mainTexture;
        // if (texture == null)
        // {
        //     Debug.LogError("Текстура не найдена на материале.");
        //     return;
        // }
        //
        // // Получаем UV-координаты и вершины меша
        // Mesh mesh = meshFilter.mesh;
        // Vector2[] uv = mesh.uv;
        // Color[] textureColors = new Color[uv.Length];
        //
        // // Проходим по всем UV-координатам и преобразуем их в пиксельные координаты текстуры
        // for (int i = 0; i < uv.Length; i++)
        // {
        //     // Преобразуем UV-координаты в координаты пикселей
        //     int x = Mathf.FloorToInt(uv[i].x * texture.width);
        //     int y = Mathf.FloorToInt(uv[i].y * texture.height);
        //
        //     // Получаем цвет пикселя
        //     textureColors[i] = texture.GetPixel(x, y);
        // }
        //
        // // Подсчет частоты появления каждого цвета
        // Dictionary<Color, int> colorFrequency = new Dictionary<Color, int>();
        // foreach (Color color in textureColors)
        // {
        //     if (colorFrequency.ContainsKey(color))
        //     {
        //         colorFrequency[color]++;
        //     }
        //     else
        //     {
        //         colorFrequency[color] = 1;
        //     }
        // }
        //
        // // Сортируем цвета по частоте и выбираем два наиболее частых
        // var sortedColors = colorFrequency.OrderByDescending(x => x.Value).Take(2).ToList();
        //
        // // Вывод двух наиболее встречающихся цветов
        // if (sortedColors.Count >= 2)
        // {
        //     Debug.Log($"1st Dominant Color: {sortedColors[0].Key}, Frequency: {sortedColors[0].Value}");
        //     Debug.Log($"2nd Dominant Color: {sortedColors[1].Key}, Frequency: {sortedColors[1].Value}");
        // }
        // else
        // {
        //     Debug.Log("Недостаточно различных цветов для анализа.");
        // }
        //
        // a.color = sortedColors[0].Key;
        // b.color = sortedColors[1].Key;
    }
}