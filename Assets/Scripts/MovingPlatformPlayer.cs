using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformPlayer : MonoBehaviour {

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.transform.parent = transform;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.transform.parent = null;
        }
    }
}
