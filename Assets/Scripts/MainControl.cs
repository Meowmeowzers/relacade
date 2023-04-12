using UnityEngine;
using UnityEngine.InputSystem;

namespace HelloWorld
{
    public class MainControl : MonoBehaviour
    {
        private Vector3 moveInput;
        private CameraControl cam;
        private TileGrid tileGrid;
        private bool zoomIn = false, zoomOut = false;

        private void Awake()
        {
            cam = GetComponent<PlayerInput>().camera.GetComponent<CameraControl>();
        }

        private void Start()
        {
            tileGrid = FindAnyObjectByType<TileGrid>();
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

#pragma warning disable IDE0060 // Remove unused parameter

        public void OnReset(InputValue value)
        {
            tileGrid.ResetWave();
        }

        public void OnHelp(InputValue value)
        {
            Debug.Log("OnHelp Button");
        }

        public void OnSave(InputValue value)

        {
            Debug.Log("OnSave Button");
        }

        public void OnExport(InputValue value)
        {
            Debug.Log("OnExport");
        }

#pragma warning restore IDE0060 // Remove unused parameter
    }
}