using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour {

    public Text CollectableUI;
    public float amountOfCollectable;

    void Start()
    {
        CollectableUI = GameObject.Find("CollectableText").GetComponent<Text>();
        amountOfCollectable = 0;
     
    }


    void OnTriggerEnter(Collider player)
    {

        if (player.gameObject.tag == "Player")
        {
            amountOfCollectable++;
            CollectableUI.text = "Collectables: " + amountOfCollectable;
            Destroy(transform.gameObject);
        }
    }



}
