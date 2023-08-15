using UnityEngine;

public class CameraRotation : MonoBehaviour{
    public Transform target;
    public Camera mainCamera;
    [Tooltip("How sensitive the touch drag to camera rotation")]
    public float touchRotateSpeed = 17.5f;
    [Tooltip("Smaller positive value means smoother rotation, 1 means no smooth apply")]
    public float slerpValue = 0.25f;


    public Vector2 swipeDirection; //swipe delta vector2
    public Quaternion cameraRot; // store the quaternion after the slerp operation
    private Touch touch;
    public float distanceBetweenCameraAndTarget;
    public float minXRotAngleTouch = -80; //min angle around x axis
    public float maxXRotAngleTouch = -5; // max angle around x axis
    
    private void Awake(){
        if (mainCamera == null)
            mainCamera = Camera.main;

        cameraRot = Quaternion.Euler(mainCamera.transform.eulerAngles.x, mainCamera.transform.eulerAngles.y, 0);
    }
    // Start is called before the first frame update
    void Start(){
        distanceBetweenCameraAndTarget = Vector3.Distance(mainCamera.transform.position, target.position);
    }

    // Update is called once per frame
    void Update(){
        if (Input.touchCount == 1){
            touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved){
                swipeDirection += touch.deltaPosition * Time.deltaTime * touchRotateSpeed;
                if (GetComponent<TutorialManager>() && GetComponent<TutorialManager>().tutorialStep == TutorialStep.Rotate)
                    GetComponent<TutorialManager>().tutorialStep = TutorialStep.Zoom;
            }
            swipeDirection.x = swipeDirection.x % 360;
            swipeDirection.y = Mathf.Clamp(swipeDirection.y, minXRotAngleTouch, maxXRotAngleTouch);
        }
    }

    private void LateUpdate(){
        Vector3 dir = new Vector3(0, 0, -distanceBetweenCameraAndTarget); //assign value to the distance between the maincamera and the target
        Quaternion newQ = Quaternion.Euler(-swipeDirection.y, swipeDirection.x, 0);

        cameraRot = Quaternion.Slerp(cameraRot, newQ, slerpValue);  //let cameraRot value gradually reach newQ which corresponds to our touch

        mainCamera.transform.position = target.position + cameraRot * dir;
        mainCamera.transform.LookAt(target.position);
    }
}