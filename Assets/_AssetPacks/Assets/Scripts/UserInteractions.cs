using System;
using UnityEngine;

namespace Riddlehouse.Core.Helpers
{
    public class UserInteractions : MonoBehaviour
    {
        private float TouchZoomSpeed = 0.1f;
        private float MouseZoomSpeed = 15f;
        private float ZoomMinBoundary = 0.1f;
        private float ZoomMaxBoundary = 179.9f;
        public Camera cam;

        private void Start()
        {
            // cam = GetComponent<Camera>();
            // if (cam != null) Debug.Log("Got my camera");
            // else Debug.Log("Error getting main camera");
        }

        private void Update()
        {
            if (Input.touchSupported)
            {
                if (Input.touchCount == 2)
                {
                    // get current touch positions
                    Touch tZero = Input.GetTouch(0);
                    Touch tOne = Input.GetTouch(1);
                    // get touch position from the previous frame
                    Vector2 tZeroPrevious = tZero.position - tZero.deltaPosition;
                    Vector2 tOnePrevious = tOne.position - tOne.deltaPosition;

                    float oldTouchDistance = Vector2.Distance (tZeroPrevious, tOnePrevious);
                    float currentTouchDistance = Vector2.Distance (tZero.position, tOne.position);

                    // get offset value
                    float deltaDistance = oldTouchDistance - currentTouchDistance;
                    Zoom (deltaDistance, TouchZoomSpeed);
                }
            }
            else
            {

                float scroll = Input.GetAxis("Mouse ScrollWheel");
                Zoom(scroll, MouseZoomSpeed);
            }
            if(cam.fieldOfView < ZoomMinBoundary) 
            {
                cam.fieldOfView = ZoomMinBoundary;
            }
            else
            if(cam.fieldOfView > ZoomMaxBoundary ) 
            {
                cam.fieldOfView = ZoomMaxBoundary;
            }

            if (Input.GetMouseButtonDown(0))
            {
                hit_position = Input.mousePosition;
                Debug.Log("Left Mouse hit");
            }
            if (Input.GetMouseButton(0))
            {
                Debug.Log("Left Mouse released");
                current_position = Input.mousePosition;
                LeftMouseDrag(hit_position, current_position);
            }
        }

        public Vector3 current_position { get; set; }

        public Vector3 hit_position { get; set; }

        private void Zoom(float deltaMagnitudeDiff, float speed)
        {
                cam.fieldOfView += deltaMagnitudeDiff * speed;
                // set min and max value of Clamp function upon your requirement
                cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, ZoomMinBoundary, ZoomMaxBoundary);
        }

        void LeftMouseDrag(Vector3 hit_position, Vector3 dragged_to_position)
        {
            //dragged_to_position.z = hit_position.z;
            // Get direction of movement.  (Note: Don't normalize, the magnitude of change is going to be Vector3.Distance(current_position-hit_position)
            // anyways.  
            Vector3 direction = Camera.main.ScreenToWorldPoint(dragged_to_position) - Camera.main.ScreenToWorldPoint(hit_position);
        
            // Invert direction to that terrain appears to move with the mouse.
            direction = direction * -1;
        
            Vector3 position = transform.position + direction;
        
            transform.forward = position;
        }
    }
}