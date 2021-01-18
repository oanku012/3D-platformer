using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargePunchScript : MonoBehaviour {

    private CharacterController controller;
    public float punchSpeed = 10.0f;
    public float punchDistanceTime = 0.5f;
    private float punchDistanceTimer;
    [HideInInspector]
    public bool hasPunched;
    private MeshRenderer punchMesh;
    private Collider punchCollider;
    //private Collider frontBlockCollider;
    private float punchDelayTimer = 0.5f;
    [HideInInspector]
    public bool punchDelayBool;
    private Animator anim;

    // Use this for initialization
    void Start () {
        controller = GetComponent<CharacterController>();
        punchCollider = GameObject.Find("ChargePunchAttack").GetComponent<Collider>();
        punchMesh = GameObject.Find("ChargePunchAttack").GetComponent<MeshRenderer>();
        punchDistanceTimer = punchDistanceTime;
        punchCollider.enabled = false;
        //frontBlockCollider = GameObject.Find("FrontBlockCollider").GetComponent<Collider>();
        //frontBlockCollider.enabled = false;
        punchMesh.enabled = false;
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        //Sets the character to a punching state
        if (Input.GetButtonDown("Fire1") && punchDelayBool == false && hasPunched == false && GetComponent<Jumper>().IsGrounded() == true && GetComponent<Jumper>().landingMovementStop == false && GetComponentInChildren<SpinAttackScript>().spinAttackBool == false && anim.enabled == true && anim.GetCurrentAnimatorStateInfo(0).IsName("Headspring") == false)
        {
            hasPunched = true;
            punchCollider.enabled = true;
            //Tried to make character invincible only from the front with a collider that blocks the impact, but it didn't work very consistently
            //frontBlockCollider.enabled = true;
            //punchMesh.enabled = true;
            //Makes character invincible while attacking
            GetComponent<HealthAndDamage>().attackInvincibility = true;
        }

        //Executes the punch motion and ends the punch after a certain duration
        if (hasPunched == true)
        {
            anim.SetBool("ChargeKick", true);
            
            punchDistanceTimer -= Time.deltaTime;
            
            if (punchDistanceTimer > 0)
            {
                controller.Move(transform.forward * punchSpeed * Time.deltaTime);
            }
            else
            {
                anim.SetBool("ChargeKick", false);
                
                
                punchMesh.enabled = false;
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("hk_side_left_A") == false)
                {
                    punchCollider.enabled = false;
                    //frontBlockCollider.enabled = false;
                    punchDelayBool = true;
                    GetComponent<HealthAndDamage>().attackInvincibility = false;
                    hasPunched = false;
                    punchDistanceTimer = punchDistanceTime;
                }

            }
        }

        

        //Adds a delay between punches
        if (punchDelayBool == true)
        {
            punchDelayTimer -= Time.deltaTime;
            if(punchDelayTimer <= 0)
            {
                punchDelayBool = false;
                punchDelayTimer = 0.5f;
            }
        }

        
    }
}
