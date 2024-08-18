using System;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class ModelToUIRenderer : MonoBehaviour
    {
        public GameObject modelPrefab; // Префаб модели, которую нужно отображать
        public RawImage targetRawImage; // Raw Image для отображения Render Texture в UI
        public Vector3 modelOffset = new Vector3(0, 0, 0); // Смещение модели относительно камеры
        public float cameraPadding = 1.1f; // Отступ камеры от модели для полной видимости
        public float objectSpacing = 1.5f; // Расстояние между объектами для предотвращения наложения

        private GameObject instantiatedModel; // Ссылка на инстанцированную модель
        private Camera renderCamera; // Автоматически созданная камера для рендеринга
        private RenderTexture renderTexture; // Созданная Render Texture
        private static int objectCount = 0; // Счётчик объектов, чтобы контролировать их размещение

        private void Update()
        {
            renderCamera.Render();
        }

        public GameObject Create()
        {
            // Инстанцируем префаб модели
            instantiatedModel = Instantiate(modelPrefab);

            // Создаем Render Texture
            renderTexture = new RenderTexture(512, 512, 16);

            // Создаем камеру для рендеринга модели
            renderCamera = new GameObject("ModelRenderCamera").AddComponent<Camera>();
            renderCamera.enabled = false; // Отключаем камеру, чтобы она не рендерила в основное окно
            renderCamera.clearFlags = CameraClearFlags.Color;
            renderCamera.backgroundColor = Color.clear; // Прозрачный фон
            renderCamera.targetTexture = renderTexture;

            // Настраиваем Raw Image для отображения Render Texture
            targetRawImage.texture = renderTexture;

            // Настраиваем позицию и масштаб модели перед камерой
           PositionModelInFrontOfCamera();

            // Увеличиваем счётчик объектов
            objectCount++;

            // Рендерим модель в Render Texture
            renderCamera.Render();

            return instantiatedModel;
        }

        void PositionModelInFrontOfCamera()
        {
            // Определяем границы модели (для установки ее размера относительно камеры)
            Bounds modelBounds = GetModelBounds(instantiatedModel);

            // Устанавливаем модель в позицию перед камерой с учетом смещения
            float modelSize = Mathf.Max(modelBounds.size.x, modelBounds.size.y, modelBounds.size.z);
            float distance = modelSize * cameraPadding;

            // Смещение камеры для корректного отображения модели
            Vector3 positionOffset = new Vector3(objectCount * objectSpacing, 0, -distance);

            // Устанавливаем камеру на правильное расстояние и смотрим на центр модели
            renderCamera.transform.position = instantiatedModel.transform.position + positionOffset;

            instantiatedModel.transform.position += new Vector3(positionOffset.x, 0) + modelOffset;

            renderCamera.transform.LookAt(instantiatedModel.transform.position);
        }

        Bounds GetModelBounds(GameObject model)
        {
            Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
                return new Bounds(model.transform.position, Vector3.zero);

            Bounds bounds = renderers[0].bounds;
            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }

            return bounds;
        }

        private void OnDestroy()
        {
            // Освобождаем ресурсы, когда объект уничтожается
            if (instantiatedModel != null)
            {
                Destroy(instantiatedModel);
            }

            if (renderTexture != null)
            {
                renderTexture.Release();
                Destroy(renderTexture);
            }

            if (renderCamera != null)
            {
                Destroy(renderCamera.gameObject);
            }

            // Уменьшаем счетчик объектов
            objectCount--;
        }
    }
}