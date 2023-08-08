using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class CameraRotation : MonoBehaviour{
    public Transform target;
    public Camera mainCamera;
    [Range(0.1f, 5f)]
    [Tooltip("How sensitive the mouse drag to camera rotation")]
    public float mouseRotateSpeed = 0.8f;
    [Range(0.01f, 100)]
    [Tooltip("How sensitive the touch drag to camera rotation")]
    public float touchRotateSpeed = 17.5f;
    [Tooltip("Smaller positive value means smoother rotation, 1 means no smooth apply")]
    public float slerpValue = 0.25f;
    public enum RotateMethod { Mouse, Touch };
    [Tooltip("How do you like to rotate the camera")]
    public RotateMethod rotateMethod = RotateMethod.Mouse;


    private Vector2 swipeDirection; //swipe delta vector2
    private Quaternion cameraRot; // store the quaternion after the slerp operation
    private Touch touch;
    private float distanceBetweenCameraAndTarget;

    public float minXRotAngleMouse = 5; //min angle around x axis
    public float maxXRotAngleMouse = 80; // max angle around x axis
    public float minXRotAngleTouch = -80; //min angle around x axis
    public float maxXRotAngleTouch = -5; // max angle around x axis

    //Mouse rotation related
    private float rotX; // around x
    private float rotY; // around y

    bool Started = false;
    private void Awake(){
        if (mainCamera == null)
            mainCamera = Camera.main;
    }
    // Start is called before the first frame update
    void Start(){
        distanceBetweenCameraAndTarget = Vector3.Distance(mainCamera.transform.position, target.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (rotateMethod == RotateMethod.Mouse)
        {
            if(Input.GetMouseButtonDown(0) && !Started)
                Started = true;
                
            if (Input.GetMouseButton(0))
            {
                rotX += -Input.GetAxis("Mouse Y") * mouseRotateSpeed; // around X
                rotY += Input.GetAxis("Mouse X") * mouseRotateSpeed;
            }

            rotX = Mathf.Clamp(rotX, minXRotAngleMouse, maxXRotAngleMouse);
        }
        else if (rotateMethod == RotateMethod.Touch)
        {
            if (Input.touchCount == 1){
                touch = Input.GetTouch(0);
                Started = true;

                if (touch.phase == TouchPhase.Moved)
                    swipeDirection += touch.deltaPosition * Time.deltaTime * touchRotateSpeed;

                swipeDirection.y = Mathf.Clamp(rotX, minXRotAngleTouch, maxXRotAngleTouch);
            }
        }
    }

    private void LateUpdate(){
        Quaternion newQ; // value equal to the delta change of our mouse or touch position
        if(!Started) {
            cameraRot = mainCamera.transform.rotation;
            newQ = mainCamera.transform.rotation;
            return;
        }

        Vector3 dir = new Vector3(0, 0, -distanceBetweenCameraAndTarget); //assign value to the distance between the maincamera and the target

        if (rotateMethod == RotateMethod.Mouse)
            newQ = Quaternion.Euler(rotX, rotY, 0); //We are setting the rotation around X, Y, Z axis respectively
        else
            newQ = Quaternion.Euler(-swipeDirection.y, swipeDirection.x, 0);

        cameraRot = Quaternion.Slerp(cameraRot, newQ, slerpValue);  //let cameraRot value gradually reach newQ which corresponds to our touch
        mainCamera.transform.position = target.position + cameraRot * dir;
        mainCamera.transform.LookAt(target.position);
    }
}