using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour {

    public float minDistance = 1.0f;
    public float maxDistance = 4.0f;
    public float smooth = 10.0f;
    Vector3 dollyDir;
    public Vector3 dollyDirAdjusted;
    public float distance;
    public float ZoomSensitivity = 4.0f;
    //Camera cam;

    private int cameraIgnore = 1 << 9;
    private int cameraIgnore2 = 1 << 13;

    void Awake()
    {
        dollyDir = transform.localPosition.normalized;
        distance = transform.localPosition.magnitude;
        //cam = GetComponent<Camera>();
        //cam.clearFlags = CameraClearFlags.SolidColor;
        cameraIgnore = ~cameraIgnore;
        cameraIgnore2 = ~cameraIgnore2;
    }

    // Update is called once per frame
    void Update () {

        Vector3 desiredCameraPos = transform.parent.TransformPoint(dollyDir * maxDistance);
        RaycastHit hit;

        if (Physics.Linecast (transform.parent.position, desiredCameraPos, out hit, cameraIgnore & cameraIgnore2))
        {
            distance = Mathf.Clamp(hit.distance * 0.87f, minDistance, maxDistance);

        } else
        {
            distance = maxDistance;
        }

        //Camera Zoom

        /*  float inputZoomIn = Input.GetAxis("Mouse ScrollWheel");


      if (inputZoomIn != 0){
          distance = inputZoomIn * distance * Time.deltaTime;
          transform.localPosition = Vector3.MoveTowards(transform.localPosition, minDistance * dollyDir, maxDistance);
          }
      */
      //This made the scene background transparent, I think...
        //cam.backgroundColor = Color.clear;

        transform.localPosition = Vector3.Lerp(transform.localPosition, dollyDir * distance, Time.deltaTime * smooth);
    }
}
