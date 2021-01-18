using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalLoadLevel : MonoBehaviour {

    void OnTriggerEnter(Collider portalCollider)
    {
        if(portalCollider.gameObject.tag == "Player")
        {
            SceneManager.LoadScene("EKA TASO");
        }
    }
}
