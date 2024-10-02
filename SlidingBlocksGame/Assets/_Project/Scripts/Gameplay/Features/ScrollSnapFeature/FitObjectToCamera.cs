using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature
{
    public class FitObjectToOrthographicCamera : MonoBehaviour
    {
        [SerializeField] private RawImage _rawImage;

        private RenderTexture _renderTexture;

        private static int counter;

        public Camera Create(GameObject target)
        {
            counter++;

            target.transform.position = new Vector3(counter * 250, 0, 0);

            _renderTexture = new RenderTexture(512, 512, 16);

            Camera camera1 = Instantiate(Resources.Load<GameObject>("Prefabs/Camera3DUI")).GetComponent<Camera>();
            
            camera1.targetTexture = _renderTexture;

            _rawImage.texture = _renderTexture;

            camera1.Render();

            Bounds bounds = GetBounds(target);

            float orthographicSize = CalculateOrthographicSize(bounds, camera1);

            camera1.orthographicSize = orthographicSize;

            float distance = bounds.center.z - bounds.extents.z + 25;
            camera1.transform.position = new Vector3(bounds.center.x, bounds.center.y, distance);

            camera1.transform.LookAt(bounds.center);
            
            camera1.Render();

            return camera1;
        }
        
        private static Bounds GetBounds(GameObject obj)
        {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            Bounds bounds = new Bounds(renderers[0].bounds.center, Vector3.zero);
            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }

            return bounds;
        }

        float CalculateOrthographicSize(Bounds bounds, Camera cam)
        {
            // Рассчитываем высоту и ширину объекта
            float objectHeight = bounds.size.y;
            float objectWidth = bounds.size.x;

            // Учитываем соотношение сторон камеры
            float aspectRatio = cam.aspect;

            // Определяем ортографический размер камеры, чтобы объект поместился по высоте и ширине
            float orthographicSize = objectHeight / 2f;
            if (objectWidth / aspectRatio > objectHeight)
            {
                orthographicSize = (objectWidth / aspectRatio) / 2f;
            }

            return orthographicSize;
        }

        private void OnDestroy()
        {
            if (_renderTexture != null)
            {
                _renderTexture.Release();
                Destroy(_renderTexture);
            }
        }
    }
}