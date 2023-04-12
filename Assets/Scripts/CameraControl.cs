using UnityEngine;

namespace HelloWorld
{
    public class CameraControl : MonoBehaviour
    {
        private Camera cam;

        private void Awake()
        {
            cam = GetComponent<Camera>();
        }

        public void SetZoom(float value)
        {
            cam.orthographicSize = value;
        }

        public void CamZoomIn()
        {
            cam.orthographicSize -= 1;
        }

        public void CamZoomOut()
        {
            cam.orthographicSize += 1;
        }

        public void MoveCam(Vector3 value)
        {
            cam.transform.position += value;
        }
    }
}