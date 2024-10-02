using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.Systems
{
    public class CameraScaler : MonoBehaviour
    {
        public Camera mainCamera;
        public float baseWidth = 1080f;  // Ширина базового разрешения (например, 1080 для 1920x1080 в портретном режиме)
        public float baseHeight = 1920f; // Высота базового разрешения (например, 1920 для 1920x1080 в портретном режиме)
        public float baseOrthographicSize = 6f; // Базовое значение orthographicSize

        void Start()
        {
            AdjustCamera();
        }

        void AdjustCamera()
        {
            float screenAspect = (float)Screen.width / (float)Screen.height;
            float baseAspect = baseWidth / baseHeight;

            if (mainCamera.orthographic)
            {
                // Для ортографической камеры
                mainCamera.orthographicSize = baseOrthographicSize;

            
                mainCamera.orthographicSize *= baseAspect / screenAspect;
            }
            else
            {
                // Для перспективной камеры (если нужно)
                float baseFOV = mainCamera.fieldOfView;
                float baseAspectRatio = baseWidth / baseHeight;
                float currentAspectRatio = (float)Screen.width / (float)Screen.height;
                mainCamera.fieldOfView = Mathf.Atan(Mathf.Tan(baseFOV * Mathf.Deg2Rad / 2f) * (baseAspectRatio / currentAspectRatio)) * 2f * Mathf.Rad2Deg;
            }
        }
    }
}