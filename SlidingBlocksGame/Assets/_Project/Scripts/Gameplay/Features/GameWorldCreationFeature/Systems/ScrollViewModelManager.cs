using UI.ThreeDimensional;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class ScrollViewModelManager : MonoBehaviour
    {
        public Camera modelRenderCamera;
        public RenderTexture[] renderTextures; // Массив Render Textures для каждой модели
        public RawImage[] rawImages; // Массив Raw Images, где отображаются модели
        public GameObject[] modelPrefabs; // Массив префабов моделей

        private GameObject currentModel;

        void Start()
        {
            // Инициализация: отображаем все модели в Raw Images
            for (int i = 0; i < modelPrefabs.Length; i++)
            {
                DisplayModel(modelPrefabs[i], renderTextures[i], rawImages[i]);
            }
        }

        void DisplayModel(GameObject modelPrefab, RenderTexture renderTexture, RawImage rawImage)
        {
            var uiObject3D = new UIObject3D();
            
            // Удаляем предыдущую модель
            if (currentModel != null)
            {
                Destroy(currentModel);
            }

            // Создаем новую модель перед камерой
            currentModel = Instantiate(modelPrefab, modelRenderCamera.transform);
            currentModel.transform.localPosition = Vector3.zero;

            // Настраиваем камеру на рендеринг в Render Texture
            modelRenderCamera.targetTexture = renderTexture;

            // Рендерим сцену
            modelRenderCamera.Render();

            // Назначаем Render Texture на Raw Image
            rawImage.texture = renderTexture;
        }
    }
}