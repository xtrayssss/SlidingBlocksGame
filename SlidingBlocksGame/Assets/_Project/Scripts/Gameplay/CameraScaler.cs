using UnityEngine;

namespace _Project.Scripts.Gameplay
{
    public class CameraScaler : MonoBehaviour
    {
        [SerializeField] private float _defaultWidth = 10f; 
        [SerializeField] private float _defaultHeight = 10f;
    
        private Camera _mainCamera;
        private float _lastScreenWidth;
        private float _lastScreenHeight;

        private void Start()
        {
            _mainCamera = GetComponent<Camera>();
            _lastScreenWidth = Screen.width;
            _lastScreenHeight = Screen.height;
            AdjustCamera();
        }

        private void Update()
        {
            if (!Mathf.Approximately(Screen.width, _lastScreenWidth) || !Mathf.Approximately(Screen.height, _lastScreenHeight))
            {
                AdjustCamera();
                _lastScreenWidth = Screen.width;
                _lastScreenHeight = Screen.height;
            }
        }

        private void AdjustCamera()
        {
            float screenRatio = (float)Screen.width / Screen.height;
            float targetRatio = _defaultWidth / _defaultHeight;

            if (screenRatio >= targetRatio)
            {
                _mainCamera.orthographicSize = _defaultHeight / 2f;
            }
            else
            {
                float differenceInSize = targetRatio / screenRatio;
                _mainCamera.orthographicSize = _defaultHeight / 2f * differenceInSize;
            }
        }
    }
}