using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class HealthAndDamage : MonoBehaviour {

    public float Health = 3;
    public Text HealthUI;
    [HideInInspector]
    public bool enemyDamage;
    public bool fallDamage;
    public float ragdollFlyTime;
    private float ragdollFlyTimer;
    public bool invincibilityFrames;

    public float KnockbackXAndZ;
    public float KnockbackY;
    private float KnockbackXAndZModifier;
    private float KnockbackYModifier;

    private Vector3 KnockBack;

    private float meshFlashTimer;
    private float meshFlashLastTime;

    private Rigidbody playerBody;
    private Rigidbody playerRootBoneRigidbody;

    private Animator anim;

    [HideInInspector]
    public Transform enemyYouCollidedWith;

    [HideInInspector]
    public bool attackInvincibility;

    Collider ragdollCollider;

    public float riseUpTime = 1.0f;
    private float riseUpTimer;

    //Playerbody
    private int layerMask = 1 << 9;
    //Player bones
    private int layerMask2 = 1 << 13;

    CharacterController controller;
    Collider playerCollider;

    private float ragdollStop = 2;
    private bool knockbackAllow;
    
    // Use this for initialization
    void Start () {
        HealthUI = GameObject.Find("HealthText").GetComponent<Text>();
        HealthUI.text = "Health: " + Health;
        anim = GetComponent<Animator>();
        ragdollFlyTimer = ragdollFlyTime;
        invincibilityFrames = false;
        enemyDamage = false;
        fallDamage = false;
        meshFlashTimer = 10f;
        meshFlashLastTime = meshFlashTimer;
        playerBody = GetComponent<Rigidbody>();
        playerRootBoneRigidbody = GameObject.Find("Root").GetComponent<Rigidbody>();
        ragdollCollider = GameObject.Find("Root").GetComponent<Collider>();
        layerMask = ~layerMask;
        layerMask2 = ~layerMask2;
        riseUpTimer = riseUpTime;

        controller = GetComponent<CharacterController>();
        playerCollider = GetComponent<Collider>();

        KnockbackXAndZModifier = KnockbackXAndZ;
        KnockbackYModifier = KnockbackY;
    }

    public bool IsRagdollGrounded()
    {
        
        return Physics.CheckCapsule(ragdollCollider.bounds.center, new Vector3(ragdollCollider.bounds.center.x, ragdollCollider.bounds.min.y -0.1f, ragdollCollider.bounds.center.z), 0.3f, layerMask & layerMask2);

    }


    // Update is called once per frame
    void Update () {
        RagdollStop();
    }
    public void HealthDamageAndKnockback()
    {
        /*if(Input.GetButtonDown("Fire3"))
        {

            anim.enabled = false;
        }*/


        //Damages the player and applies knockback when getting hit by an enemy
         if (enemyDamage == true && invincibilityFrames == false && attackInvincibility == false && Health >0)
        {
            Health--;

            //Detects if enemy is behind player when attacking, not necessary at this time
            /*if (Vector3.Dot(enemyYouCollidedWith.forward, transform.forward)>0)
            {
                //KnockbackXAndZModifier += 7;
                //KnockbackYModifier += 70;
                Debug.Log("Is behind back");
            }*/
            
               


            KnockBack = new Vector3((transform.position.x - enemyYouCollidedWith.position.x) * KnockbackXAndZModifier, (transform.position.y - enemyYouCollidedWith.position.y) + KnockbackYModifier, (transform.position.z - enemyYouCollidedWith.position.z) * KnockbackXAndZModifier);
            //GetComponent<Jumper>().moveDirection = KnockBack;



            anim.enabled = false;
            invincibilityFrames = true;
            
            KnockbackXAndZModifier = KnockbackXAndZ;
            KnockbackYModifier = KnockbackY;

        }

        //Makes the player ragdoll and temporarily invincible when getting hit
        if (invincibilityFrames == true)
        {
            
            
            ragdollFlyTimer -= Time.deltaTime;

            if (knockbackAllow == true)
            {
                playerRootBoneRigidbody.velocity = KnockBack;
                knockbackAllow = false;

            }

            //A flashing effect for the old Max character when taking damage
            /*if (meshFlashTimer == meshFlashLastTime)
            {
                
                /*GameObject.Find("CC_Base_Body").GetComponent<SkinnedMeshRenderer>().enabled = false;
                GameObject.Find("Boxers").GetComponent<SkinnedMeshRenderer>().enabled = false;
                GameObject.Find("CC_Base_Eye").GetComponent<SkinnedMeshRenderer>().enabled = false;
                GameObject.Find("CC_Base_Teeth").GetComponent<SkinnedMeshRenderer>().enabled = false;
                GameObject.Find("CC_Base_Tongue").GetComponent<SkinnedMeshRenderer>().enabled = false;
                meshFlashTimer -= Time.deltaTime;
            }
            else if(meshFlashTimer < (meshFlashLastTime - 0.1f))
            {
                /*GameObject.Find("CC_Base_Body").GetComponent<SkinnedMeshRenderer>().enabled = true;
                GameObject.Find("Boxers").GetComponent<SkinnedMeshRenderer>().enabled = true;
                GameObject.Find("CC_Base_Eye").GetComponent<SkinnedMeshRenderer>().enabled = true;
                GameObject.Find("CC_Base_Teeth").GetComponent<SkinnedMeshRenderer>().enabled = true;
                GameObject.Find("CC_Base_Tongue").GetComponent<SkinnedMeshRenderer>().enabled = true;
                meshFlashTimer -= Time.deltaTime;
                if (meshFlashTimer < (meshFlashLastTime - 0.2f))
                {
                    meshFlashLastTime = meshFlashTimer;
                }
            }
            else
            {
                meshFlashTimer -= Time.deltaTime;
            }*/
            
            

            
            //Ragdollflytimer was added so that character doesn't immediately detect being grounded at the start of knockback, might not be needed anymore
            if (ragdollFlyTimer < 0 && IsRagdollGrounded())
            {
                

                //Makes the character rise up after a certain amount of time while lying on the ground
                riseUpTimer -= Time.deltaTime;
                if(riseUpTimer < 0)
                {
                    transform.position = GameObject.Find("Root").transform.position;
                    //GetComponent<Jumper>().moveDirection = Vector3.zero;
                    invincibilityFrames = false;
                    
                    anim.enabled = true;
                    anim.Play("Headspring");
                    riseUpTimer = riseUpTime;
                    ragdollFlyTimer = ragdollFlyTime;
                    
                    
                    
                }
                
                /*GameObject.Find("CC_Base_Body").GetComponent<SkinnedMeshRenderer>().enabled = true;
                GameObject.Find("Boxers").GetComponent<SkinnedMeshRenderer>().enabled = true;
                GameObject.Find("CC_Base_Eye").GetComponent<SkinnedMeshRenderer>().enabled = true;
                GameObject.Find("CC_Base_Teeth").GetComponent<SkinnedMeshRenderer>().enabled = true;
                GameObject.Find("CC_Base_Tongue").GetComponent<SkinnedMeshRenderer>().enabled = true;*/
            }
        }
        enemyDamage = false;

        HealthUI.text = "Health: " + Health;

        if (Health <= 0)
        {
                //anim.SetTrigger("Dead");
                anim.enabled = false;
                GetComponent<Jumper>().enabled = false;
                GetComponent<Movement>().enabled = false;
                GetComponent<ChargePunchScript>().enabled = false;
                GameObject.Find("Spin-Attack").GetComponent<SpinAttackScript>().enabled = false;
                
            
            
            

        }

        
    }

    //Freezes character temporarily when turning into a ragdoll in order to stop character from flying unintentionally, also moved all the general changes when ragdolling here
    public void RagdollStop()
    {

        if (anim.enabled == false)
        {
            //transform.position = GameObject.Find("Root").transform.position;
            anim.SetFloat("InputMagnitude", 0);
            GetComponent<Movement>().enabled = false;
            GameObject.Find("Camerabase").GetComponent<Cameracontroller>().CameraFollowObj = GameObject.Find("Root");
            controller.enabled = false;
            playerCollider.enabled = false;
            var childrenRigidbodies = GetComponentsInChildren<Rigidbody>();
            if (ragdollStop == 2)
            {

                foreach (var childRigidbody in childrenRigidbodies)
                {
                    childRigidbody.constraints = RigidbodyConstraints.FreezeAll;
                }
                ragdollStop--;
            }
            else if (ragdollStop ==1)
            {
                foreach (var childRigidbody in childrenRigidbodies)
                {
                    childRigidbody.constraints = ~RigidbodyConstraints.FreezeAll;
                }
                ragdollStop--;
                knockbackAllow = true;
            }

        }
        else if (ragdollStop != 2)
        {
            ragdollStop = 2;
            GameObject.Find("Camerabase").GetComponent<Cameracontroller>().CameraFollowObj = GameObject.Find("CameraFollow");
            GetComponent<Movement>().enabled = true;
            controller.enabled = true;
            playerCollider.enabled = true;
        }
    }
}
