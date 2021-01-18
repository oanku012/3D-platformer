using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAttackScript : MonoBehaviour {

    private Collider thisCollider;
    private float spinAttackTimer;

    [HideInInspector]
    public bool spinAttackBool = false;

    private float spinAttackDelayTimer;
    private bool spinAttackDelayBool;
    private MeshRenderer thisMesh;
    private Animator anim;

    private Vector3 spin;
    private Vector3 spinStopRotation;

	// Use this for initialization
	void Start () {
        thisCollider = GetComponent<Collider>();
        thisCollider.enabled = false;
        spinAttackBool = false;
        thisMesh = GetComponent<MeshRenderer>();
        thisMesh.enabled = false;
        anim = GetComponentInParent<Animator>();
        spin = Vector3.zero;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        //Starts the attack
		if (Input.GetButtonDown("Fire2") && spinAttackBool == false && spinAttackDelayBool == false && GetComponentInParent<Jumper>().landingMovementStop == false && GetComponentInParent<ChargePunchScript>().hasPunched == false && GetComponentInParent<Jumper>().hasStomped == false && anim.enabled == true && anim.GetCurrentAnimatorStateInfo(0).IsName("Headspring") == false)
        {
            GetComponentInParent<Jumper>().hasStomped = false;
            anim.SetBool("HurricaneKick", true);
            thisCollider.enabled = true;
            spinAttackBool = true;
            spinAttackTimer = 0.3f;
            //thisMesh.enabled = true;
            GetComponentInParent<HealthAndDamage>().attackInvincibility = true;
            spin = Vector3.zero;
            spinStopRotation = transform.parent.forward;
        }

        //Executes the spinning and Ends the spin-attack after a certain duration, also makes character face forward after spin has ended
        if (spinAttackBool == true)
        {
            
            transform.parent.rotation = Quaternion.Euler(spin);
            spin.y -= 32f;
            spinAttackTimer -= Time.deltaTime;
            if (spinAttackTimer <= 0)
            {
                anim.SetBool("HurricaneKick", false);
                spinAttackBool = false;
                thisCollider.enabled = false;
                //thisMesh.enabled = false;
                spinAttackDelayBool = true;
                spinAttackDelayTimer = 0.5f;
                GetComponentInParent<HealthAndDamage>().attackInvincibility = false;
                transform.parent.rotation = Quaternion.LookRotation(spinStopRotation, Vector3.up);
                
            }
        }

        //Creates a delay between spin-attacks
        if (spinAttackDelayBool == true)
        {
            spinAttackDelayTimer -= Time.deltaTime;
            if(spinAttackDelayTimer <= 0)
            {
                spinAttackDelayBool = false;
               
            }
        }
	}
}
