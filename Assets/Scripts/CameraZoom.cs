using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour{
    
    bool wasZoomingLastFrame = false;
    Vector2[] lastZoomPositions;
    public float ZoomSpeedTouch;
    public float MaxFoV;
    public float MinFoV;

    void Update(){
        HandleMouse();
        HandleTouch();
    }
    void HandleMouse(){
        ZoomCamera(Input.mouseScrollDelta.y, ZoomSpeedTouch*50);
    }
    void HandleTouch()
    {
        if (Input.touchCount == 2){
                Vector2[] newPositions = new Vector2[] { Input.GetTouch(0).position, Input.GetTouch(1).position };
                if (!wasZoomingLastFrame)
                {
                    lastZoomPositions = newPositions;
                    wasZoomingLastFrame = true;
                }
                else
                {
                    // Zoom based on the distance between the new positions compared to the 
                    // distance between the previous positions.
                    float newDistance = Vector2.Distance(newPositions[0], newPositions[1]);
                    float oldDistance = Vector2.Distance(lastZoomPositions[0], lastZoomPositions[1]);
                    float offset = newDistance - oldDistance;

                    ZoomCamera(offset, ZoomSpeedTouch);

                    lastZoomPositions = newPositions;
                }

        }else{
                wasZoomingLastFrame = false;
        }
    }

    void ZoomCamera(float offset, float speed){
        if (offset == 0)
            return;

        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - (offset * speed), MaxFoV, MinFoV);
    }
}
