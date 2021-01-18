using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilingScaleTowerFloor : MonoBehaviour {

    private MeshRenderer thisMesh;



    // Use this for initialization
    void Start()
    {
        thisMesh = GetComponent<MeshRenderer>();
        thisMesh.material.mainTextureScale = new Vector2(4f , 4f);
    }
}
