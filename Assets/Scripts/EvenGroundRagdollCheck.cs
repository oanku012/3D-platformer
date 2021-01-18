using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvenGroundRagdollCheck : MonoBehaviour {

    Vector3 hitNormal;

    public bool IsRagdollOnEvenGround()
    {
        return Vector3.Angle(Vector3.up, hitNormal) <= 50;
    }

    void OnCollisionEnter(Collision collision)
    {
        hitNormal = collision.contacts[0].normal;
        //Copied from unity's documentation, worth keeping in mind
        //if (collision.relativeVelocity.magnitude > 2)
            //audioSource.Play();
    }
}
