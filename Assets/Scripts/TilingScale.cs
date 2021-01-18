using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilingScale : MonoBehaviour {
    private MeshRenderer thisMesh;
    
  

	// Use this for initialization
	void Start () {
        thisMesh = GetComponent<MeshRenderer>();
        thisMesh.material.mainTextureScale = new Vector2(transform.localScale.x, transform.localScale.z);
    }
	
	
}
