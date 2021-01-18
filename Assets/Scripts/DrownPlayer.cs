using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrownPlayer : MonoBehaviour {

    void OnTriggerEnter(Collider Player)
    {
        if (Player.gameObject.tag == "Player" || Player.gameObject.tag == "PlayerBoneRoot")
        {
            GameObject.Find("Robot Kyle").GetComponent<HealthAndDamage>().Health = 0;
        }
    }
}
