using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class EnemyDamageAndDeath : MonoBehaviour
{
    public float enemyHealth;
    private Vector3 playerPosition;
    public bool enemyWasDamaged = false;
    private float damagedTimer;

    public float enemyKnockbackXZ = 5.0f;
    public float enemyKnockbackY = 8.0f;
    private Vector3 enemyKnockback;
    public float gravity = 8.0f;

    Rigidbody enemyRigidbody;
    ForceMode mode;

    Collider enemyCollider;
    Collider enemyRagdollCollider;
    //Ignores enemy's own body
    private int layerMask1 = 1 << 11;
    private int layerMask2 = 1 << 12;

    Animator anim;



    private void Start()
    {
        enemyRigidbody = GetComponent<Rigidbody>();
        enemyCollider = GetComponentInChildren<Collider>();

        anim = GetComponent<Animator>();
        layerMask1 = ~layerMask1;
        layerMask2 = ~layerMask2;

    }

    public bool IsGrounded()
    {
        
        return Physics.CheckCapsule(enemyCollider.bounds.center, new Vector3(enemyCollider.bounds.center.x, enemyCollider.bounds.min.y - 0.1f, enemyCollider.bounds.center.z), 0.3f, layerMask1 & layerMask2);

    }

    public bool IsRagdollGrounded()
    {
        var childrenColliders = GetComponentsInChildren<Collider>();
        foreach (var childCollider in childrenColliders)
        {
            if(childCollider.name == "spine1_hiResSpine2")
            {
                enemyRagdollCollider = childCollider;
            }
        }
        return Physics.CheckCapsule(enemyRagdollCollider.bounds.center, new Vector3(enemyRagdollCollider.bounds.center.x, enemyRagdollCollider.bounds.min.y, enemyRagdollCollider.bounds.center.z), 0.3f, layerMask1 & layerMask2);

        
    }


    private void FixedUpdate()
    {
        

        if (IsGrounded())
        {

        
        Debug.Log("Enemy is grounded");

    }
        else
        {
            Debug.Log("Enemy is not grounded");
        }

        if (enemyWasDamaged == true) {

            enemyRigidbody.isKinematic = false;
            damagedTimer -= Time.deltaTime;
            GetComponent<NavMeshAgent>().enabled = false;
            GetComponent<BasicEnemyAI>().enabled = false;

            var childrenColliders = GetComponentsInChildren<Collider>();
            foreach (var childCollider in childrenColliders)
            {
                if (childCollider.name == "TigerCollider")
                {
                    childCollider.enabled = false;
                }
            }



            anim.SetBool("isAttacking", false);
            anim.SetBool("isAlert", false);
            anim.SetBool("isIdle", true);
            anim.SetBool("isRunning", false);
            anim.SetBool("isWalking", false);
            anim.enabled = false;
            

            if (damagedTimer < 0 && IsRagdollGrounded() && enemyHealth>0)
            {
                
                
                GetComponent<NavMeshAgent>().enabled = true;
                GetComponent<BasicEnemyAI>().enabled = true;
                GetComponentInChildren<Collider>().enabled = true;
                enemyWasDamaged = false;
                enemyRigidbody.isKinematic = true;
                enemyHealth--;
                anim.enabled = true;

                var childrenRigidbodies = GetComponentsInChildren<Rigidbody>();
                foreach (var childRigidbody in childrenRigidbodies)
                {
                    childRigidbody.isKinematic = true;
                    //childRigidbody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
                    
                    childRigidbody.useGravity = false;
                }

                enemyRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

                var childrenTransforms = GetComponentsInChildren<Transform>();
                foreach (var childTransform in childrenTransforms)
                {

                    if (childTransform.name == ("spine1_hiResSpine1")){
                        transform.position = childTransform.transform.position;
                        transform.rotation = childTransform.transform.rotation;
                    }
                }

                foreach (var childCollider in childrenColliders)
                {
                    if (childCollider.name == "TigerCollider")
                    {
                        childCollider.enabled = true;
                    }
                }

            }
        }

        if (enemyHealth <= 0)
        {
            Destroy(transform.gameObject);
        }


        /*if(enemyKnockback.y > -20)
        {
            enemyKnockback.y -= gravity;
        }*/
    }



    void OnTriggerEnter(Collider EnemyKiller)
    {
        //Kills the enemy if an enemy killing collider hits, also causes player to bounce up from jumping on the enemy
        if ((EnemyKiller.gameObject.tag == "KillerFeet" || EnemyKiller.gameObject.tag == "EnemyKiller") && enemyWasDamaged == false)
        {
            
            if (EnemyKiller.gameObject.tag == "KillerFeet")
            {
                float BounceOffEnemy = GameObject.Find("Robot Kyle").GetComponent<Jumper>().jumpspeed;

                GameObject.Find("Robot Kyle").GetComponent<Jumper>().moveDirection.y = BounceOffEnemy;
            }

            //Causes enemy to be knocked back when hit
            enemyRigidbody.isKinematic = false;
            enemyRigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationX;
            enemyRigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationZ;

            var childrenRigidbodies = GetComponentsInChildren<Rigidbody>();
            foreach (var childRigidbody in childrenRigidbodies)
            {
                childRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionX;
                childRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionZ;
                childRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionY;
                childRigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationX;
                childRigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationZ;
                childRigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationY;
                childRigidbody.isKinematic = false;
                childRigidbody.useGravity = true;
            }


            playerPosition = GameObject.Find("Robot Kyle").transform.position;
            enemyKnockback = new Vector3((transform.position.x - playerPosition.x) * enemyKnockbackXZ, enemyKnockbackY, (transform.position.z - playerPosition.z) * enemyKnockbackXZ);
            foreach (var childRigidbody in childrenRigidbodies)
            {
                if (childRigidbody.name == ("spine1_hiResSpine1"))
                {
                    childRigidbody.velocity = enemyKnockback;
                }
            }

            //Add rigidbody TSMGWorldjoint for this
            /*var childrenTransforms = GetComponentsInChildren<Transform>();
            foreach (var childTransform in childrenTransforms)
            {

                if (childTransform.name == ("TSMGWorldJoint"))
                {
                    childTransform.GetComponent<Rigidbody>().velocity = enemyKnockback;
                }
            }*/
            //enemyRigidbody.velocity = enemyKnockback;


            enemyWasDamaged = true;
            damagedTimer = 2.0f;
           
        }
        //Inflicts damage on the player
        else if (EnemyKiller.gameObject.tag == "Player" && anim.enabled == true)
        {
            GameObject.Find("Robot Kyle").GetComponent<HealthAndDamage>().enemyYouCollidedWith = transform;
            GameObject.Find("Robot Kyle").GetComponent<HealthAndDamage>().enemyDamage=true;
            GetComponent<BasicEnemyAI>().iDamagedThePlayer = true;
        }
        /*else if (EnemyKiller.gameObject.tag == "Ground")
        {
            enemyWasDamaged = false;
            enemyKnockback = Vector3.zero;
            enemyRigidbody
        }*/
    }

    
}
