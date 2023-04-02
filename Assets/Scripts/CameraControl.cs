using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Genesis
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
    }
}
