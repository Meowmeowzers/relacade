using UnityEngine;
using UnityEngine.InputSystem;

namespace Genesis
{
    public class MainControl : MonoBehaviour
    {
        private Vector3 moveInput;
        private CameraControl cam;
        private bool zoomIn = false, zoomOut = false;

        private void Awake()
        {
            cam = GetComponent<PlayerInput>().camera.GetComponent<CameraControl>();
        }

        private void FixedUpdate()
        {
            cam.MoveCam(moveInput);
            if (zoomIn && !zoomOut)
            {
                cam.CamZoomIn();
            }
            else if (!zoomIn && zoomOut)
            {
                cam.CamZoomOut();
            }
            zoomIn = false;
            zoomOut = false;
        }

        public void OnMove(InputValue value)
        {
            moveInput = value.Get<Vector2>();
        }

        public void OnCamZoomIn(InputValue value)
        {
            if (value.isPressed)
            {
                zoomIn = true;
            }
            else
            {
                zoomIn = false;
            }
        }

        public void OnCamZoomOut(InputValue value)
        {
            if (value.isPressed)
            {
                zoomOut = true;
            }
            else
            {
                zoomOut = false;
            }
        }
    }
}