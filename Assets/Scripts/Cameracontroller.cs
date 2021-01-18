using UnityEngine;
using System.Collections;

public class Cameracontroller : MonoBehaviour
{
    public float turnSpeed = 4.0f;
    public GameObject CameraFollowObj;

    Vector3 FollowPosition;
    public float Clampangle = 80f;
    public float InputSensitivity = 150.0f;

    public GameObject CameraObject;
    public GameObject PlayerObject;

    public float CamdistanceXtoPlayer;
    public float CamdistanceYtoPlayer;
    public float CamdistanceZtoPlayer;

    public float MouseX;
    public float MouseY;

    public float FinalInputX;
    public float FinalInputZ;

    public float SmoothX;
    public float SmoothY;

    private float rotY = 0.0f;
    private float rotX = 0.0f;

    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        //We setup the rotation of the sticks here
        float inputX = Input.GetAxis("RightStickHorizontal");
        float inputZ = Input.GetAxis("RightStickVertical");
        MouseX = Input.GetAxis("Mouse X");
        MouseY = Input.GetAxis("Mouse Y");
        FinalInputX = inputX + MouseX;
        FinalInputZ = inputZ + MouseY;

        rotY += FinalInputX * InputSensitivity * Time.deltaTime;
        rotX += FinalInputZ * InputSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -Clampangle, Clampangle);

        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        transform.rotation = localRotation;

        
    }

    void LateUpdate()
    {
        CameraUpdater();  
    }

    void CameraUpdater()
    {
        // set the target object to follow
        Transform target = CameraFollowObj.transform;

        //move towards the game object that is the target
        float step = turnSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);

    }
}