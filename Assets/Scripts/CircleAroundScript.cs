using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleAroundScript : MonoBehaviour {

    public float RotateSpeed = 2f;
    public float Radius = 0.5f;

    private Vector3 _centre;
    private float _angle;

    private void Start()
    {
        _centre = transform.position;
    }

    private void Update()
    {
        

        _angle += RotateSpeed * Time.deltaTime;

        //Interesting effect
        //var offset = new Vector3(Mathf.Tan(_angle), Mathf.Cos(_angle)) * Radius;
        var offset = new Vector3(Mathf.Sin(_angle) * Radius, 0, Mathf.Cos(_angle) * Radius);
        transform.position = _centre + offset;
    }
}
