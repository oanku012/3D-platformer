using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : MonoBehaviour {

    public float jumpspeed = 8.0f;
    public float gravity = 20.0f;
    public float jumpCount = 2;
    [HideInInspector]
    public float jumpChanger = 0;

    public CharacterController controller;
    public Vector3 moveDirection = Vector3.zero;

    //Needs to always be 2
    [HideInInspector]
    public float fallFromEdge = 2;

    private float jumpTimer;
    private bool hasJumped = false;

    private Collider playerCollider;
    //Playerbody
    private int layerMask1 = 1 << 9;
    //Ignoreplayerbody
    private int layerMask2 = 1 << 8;
    //Enemy
    private int layerMask3 = 1 << 11;
    //PlayerBones
    private int layerMask4 = 1 << 13;

    //WallJump
    bool hasWallJumped;
    bool collidingWithWall;
    float rotationTimes;

    private Animator anim;

    //Slope sliding system variables
    //private bool isOnEvenGround;
    public float slideFriction = 0.1f;
    private Vector3 hitNormal; //Orientation of the slope
    private float runSpeed;
    private Vector3 oppositeMoveDirection;
    private float speedDecelarator;
    private Vector3 currentMoveSpeed;
    private bool isSliding;
    private bool hasFallenOnGround = true;

    //Changing slope limit for stairs
    private float defaultSlopeLimit;
    public bool isOnStairs;

    //Jump animation
    public float jumpAnimationSpeed = 10f;
    //private Animation JumpingStart;
    //private Animation JumpingMid;
    //private Animation JumpingEnd;

    //Stomp attack variables
    private Collider killerFeetCollider;
    public bool hasStomped;
    private bool fallDamageEnabled;
    private float fallDamageTimer;
    private float landingMovementStopTimer;
    [HideInInspector]
    public bool landingMovementStop;

    //Variables for stopping movement after taking fall damage
    private bool hasTakenFallDamage;
    private float fallDamageAnimationTimer;

    private Rigidbody rootBoneRigidbody;

    // Use this for initialization
    void Start () {
        controller = GetComponent<CharacterController>();
        jumpChanger = jumpCount;
        playerCollider = GetComponent<Collider>();
        layerMask1 = ~layerMask1;
        layerMask2 = ~layerMask2;
        layerMask3 = ~layerMask3;
        layerMask4 = ~layerMask4;
        jumpTimer = 1.0f;

        hasWallJumped = false;
        collidingWithWall = false;

        anim = GetComponent<Animator>();
        runSpeed = GetComponent<Movement>().runSpeed;
        defaultSlopeLimit = controller.slopeLimit;
        isOnStairs = false;
        //JumpingStart["Jumping1"].speed = jumpAnimationSpeed;
        //JumpingMid["Jumping2"].speed = jumpAnimationSpeed;
        //JumpingEnd["Jumping3"].speed = jumpAnimationSpeed;

        killerFeetCollider = GameObject.Find("KillerFeet").GetComponent<Collider>();
        killerFeetCollider.enabled = false;
        hasStomped = false;
        fallDamageEnabled = true;

        rootBoneRigidbody = GameObject.Find("Root").GetComponent<Rigidbody>();
    }

    public bool IsGrounded()
    {
        //IsGrounded ignores Playerbody layer(ignores the player's own collider so you don't get constant IsGrounded) and IgnorePlayerBody(I don't remember why I added this) 
        return Physics.CheckCapsule(playerCollider.bounds.center, new Vector3(playerCollider.bounds.center.x, playerCollider.bounds.min.y - 0.1f, playerCollider.bounds.center.z), 0.3f, layerMask1 & layerMask2 & layerMask3 & layerMask4);
       
    }

    public bool HitCeiling()
    {
        return Physics.CheckCapsule(playerCollider.bounds.center, new Vector3(playerCollider.bounds.center.x, playerCollider.bounds.max.y - 0.2f, playerCollider.bounds.center.z), 0.3f, layerMask1 & layerMask2 & layerMask3 & layerMask4);
    }

    public bool IsOnEvenGround()
    {
        return Vector3.Angle(Vector3.up, hitNormal) <= controller.slopeLimit;
    }

    // Update is called once per frame
    void Update () {
        GetComponent<HealthAndDamage>().HealthDamageAndKnockback();
        //Airstomp doesn't stop character if you do it after walljumping, needs fix
        AirStompAttack();
        JumperFunction();
        
        if (IsGrounded() == true)
        {
            Debug.Log("Player is grounded");

        }
        else
        {
            Debug.Log("Player is not grounded");
        }
    }


    void JumperFunction()
    {
        
        //Adds fall damage
        if (moveDirection.y < -20 && IsGrounded() == true && fallDamageEnabled == true)
        {
            //anim.SetTrigger("FallDamage");
            //rootBoneRigidbody.isKinematic = false;
            //rootBoneRigidbody.constraints = ~RigidbodyConstraints.FreezeAll;
            GetComponent<HealthAndDamage>().Health--;
            GetComponent<Movement>().enabled = false;
            moveDirection.z = 0;
            moveDirection.x = 0;
            anim.SetFloat("InputMagnitude", 0);
            anim.enabled = false;
            hasTakenFallDamage = true;
            fallDamageAnimationTimer = 1.7f;
            GameObject.Find("Camerabase").GetComponent<Cameracontroller>().CameraFollowObj = GameObject.Find("Root");

        }
        else if(IsGrounded() == true)
        {
            fallDamageEnabled = true;
        }

        //Lets player move again after animation/ragdoll has ended
        if(hasTakenFallDamage == true)
        {
            fallDamageAnimationTimer -= Time.deltaTime;
            //transform.position = GameObject.Find("Root").transform.position;
            if (fallDamageAnimationTimer < 0 && GetComponent<HealthAndDamage>().IsRagdollGrounded() && GameObject.Find("Root").GetComponent<Rigidbody>().velocity.magnitude < 0.2)
            {
                GetComponent<Movement>().enabled = true;
                hasTakenFallDamage = false;
                anim.enabled = true;
                GameObject.Find("Camerabase").GetComponent<Cameracontroller>().CameraFollowObj = GameObject.Find("CameraFollow");
                transform.position = GameObject.Find("Root").transform.position;
                anim.Play("Headspring");
                //rootBoneRigidbody.isKinematic = true;
                //rootBoneRigidbody.constraints = RigidbodyConstraints.FreezeAll;
                moveDirection = Vector3.zero;
            }
        }

        //Resets the amount of jumps you can do when you hit the ground, ADD ISONEVENGROUND ON THE IF RIGHT BELOW THIS TO MAKE CHARACTER RAGDOLL FROM SLOPE
        if (IsGrounded() )
        {

            
            if (moveDirection.y < -15)
            {
                moveDirection.y = 0;
            }
            collidingWithWall = false;
            if (hasJumped == false && IsOnEvenGround())
            {
                jumpChanger = jumpCount;
                anim.SetBool("HitGround", true);
                anim.SetBool("Jump", false);
                anim.SetBool("Fall", false);
            }
            fallFromEdge = 2;
            //hitNormal = Vector3.zero;
            
        }

        
     
        //Applies correct gravity when you fall from edge 
        if (IsGrounded() == false && jumpChanger == jumpCount && moveDirection.y <0)
        {
            //Falling animation
            anim.SetBool("Fall", true);
            anim.SetBool("HitGround", false);
            fallFromEdge--;
            if (fallFromEdge == 1 && GetComponent<HealthAndDamage>().invincibilityFrames == false)  {
                moveDirection.y = 0;
            }
            moveDirection.y -= gravity * Time.deltaTime;
            
        }
        else
        {
            
                moveDirection.y -= gravity * Time.deltaTime;
            
        }
     
        if (jumpChanger > jumpCount)
        {
            jumpChanger = jumpCount;
        }


        //Lets the player jump and changes the amount of jumps you can do based on whether you've already jumped and if you're in the air
        if (Input.GetButtonDown("Jump") && jumpChanger > 0 && collidingWithWall == false && hasTakenFallDamage == false && anim.GetCurrentAnimatorStateInfo(0).IsName("Headspring") == false && anim.enabled== true)
        {

            //Makes you unable to change move direction after a double jump from a walljump
            //if (hasWallJumped == false)
            //{
                moveDirection = Vector3.zero;
                GetComponent<Movement>().enabled = true;
            //}
            if ((IsGrounded() == false && jumpChanger == jumpCount) || (IsOnEvenGround() == false && jumpChanger == jumpCount))
            {
                jumpChanger--;
                anim.SetTrigger("Double Jump");
            }
            else if(IsGrounded() == false && jumpChanger < jumpCount)
            {
                anim.SetTrigger("Double Jump");
            }
            moveDirection.y = jumpspeed;
            jumpChanger--;
            
            hasJumped = true;
        }

        //Fixes an issue I had with the new isGrounded system that didn't let me jump
        if (hasJumped == true)
        {
            anim.SetBool("Jump", true);
            anim.SetBool("HitGround", false);
            jumpTimer -= Time.deltaTime * 4;
            if(jumpTimer <= 0)
            {
                
                anim.SetBool("Fall", false);
                anim.SetBool("Jump", false);
                hasJumped = false;
                jumpTimer = 1.0f;
            }
        }

       
        

        if (hasWallJumped == true)
        {
            //Rotates character when walljumping, made this so the walljump animation looks better
            if (rotationTimes > 0)
            {
                    transform.Rotate(Vector3.up, 10);
                    rotationTimes -= 10;
                
                
            }

            collidingWithWall = false;
                //Stops walljump momentum when hitting ground
                if (IsGrounded() == true)
                {
                    moveDirection.x = 0;
                    moveDirection.z = 0;
                    hasWallJumped = false;
                    GetComponent<Movement>().enabled = true;
                    
                }
                
                
            
        }
        
        //SLOPE SLIDEY STUFF
        
        if (!IsOnEvenGround() && IsGrounded() && hasWallJumped == false && GetComponent<HealthAndDamage>().invincibilityFrames == false)
        {
            
            moveDirection.x += ((1f - hitNormal.y) * hitNormal.x * (slideFriction));
            moveDirection.z += ((1f - hitNormal.y) * hitNormal.z * (slideFriction));
            isSliding = true;
            Debug.Log("Is Not On Even Ground");
            
            
            
        }
        else if(GetComponent<HealthAndDamage>().invincibilityFrames == false && hasWallJumped == false)
        {
            //Decelerates player when they hit even ground
            if (controller.velocity.magnitude > 1 && isSliding == true)
            {
                moveDirection.x /= 1.06f;
                moveDirection.z /= 1.06f;
            }
            else if(isSliding == true)
            {
                moveDirection.x = 0;
                moveDirection.z = 0;
                isSliding = false;
                
            }
            
                

            Debug.Log("Is On Even Ground");
            
        }

        //Ragdoll from sliding on hill, too buggy

        /*if (GameObject.Find("Root").GetComponent<Rigidbody>().velocity.magnitude < 0.2 && isSliding == true && anim.enabled == false && GameObject.Find("Root").GetComponent<Rigidbody>().velocity.magnitude != 0)
         {
             transform.position = GameObject.Find("Root").transform.position;
             controller.Move(Vector3.zero);
             anim.enabled = true;
             anim.Play("Headspring");
             isSliding = false;
             hasFallenOnGround = true; 
         else if (isSliding == true && controller.velocity.magnitude > 8)
        {

            anim.enabled = false;
            controller.enabled = false;
        }
        else if(GameObject.Find("Root").GetComponent<Rigidbody>().velocity.magnitude > 0.2)
        {
            hasFallenOnGround = false;
        }
       else if(GameObject.Find("Root").GetComponent<Rigidbody>().velocity.magnitude < 0.2)
        {
            hasFallenOnGround = true;
        }*/
        

        //Debug.Log(controller.velocity);

        if (!IsGrounded())
        {
            anim.SetBool("HitGround", false);
            anim.SetBool("Fall", true);
            //hitNormal = Vector3.zero;
        }

        //Changes slope limit for stairs, THIS STILL NEEDS FIXING SOMETIMES CAN'T ENTER STAIRS
        /*if (isOnStairs == true)
        {
            controller.slopeLimit = 50;
        }
        else if (isOnStairs == false && controller.slopeLimit != defaultSlopeLimit)
        {
            controller.slopeLimit = defaultSlopeLimit;
        }*/

        //Stops the player character's jump momentum if hitting a ceiling
       if (HitCeiling())
        {
            moveDirection.y = 0;
            moveDirection.y -= gravity * Time.deltaTime;
        }

        //Spin-attack floating
        if(GameObject.Find("Spin-Attack").GetComponent<SpinAttackScript>().spinAttackBool == true && IsGrounded() == false)
        {
            moveDirection.y = 0;
        }

        if (anim.enabled == true)
        {
            controller.Move(moveDirection * Time.deltaTime);
        }
    }

    //Allows walljumping
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
      
        //Allows walljumping only from walls with "Wall" tag
        //if (hit.gameObject.tag == "Wall")
        //{

            collidingWithWall = true;

            if (IsGrounded() == false && hit.normal.y < 0.1f)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    anim.SetTrigger("WallJump");
                    rotationTimes = 180;
                    //Lets you always do a double jump after walljump
                    //jumpChanger= jumpCount-1;
                    anim.SetFloat("InputMagnitude", 0);
                    GetComponent<Movement>().enabled = false;

                    moveDirection = new Vector3(hit.normal.x * 4, jumpspeed, hit.normal.z * 4);
                    hasWallJumped = true;


                }
            }
        //}
        //Slope sliding thingy
        if (hit.gameObject.tag != "Wall")
        {
            hitNormal = hit.normal;
        }

        if (hit.gameObject.tag == "Stairs")
        {
            isOnStairs = true;
        }
        else
        {
            isOnStairs = false;
        }

    }

    void AirStompAttack()
    {
        if(IsGrounded() == false && Input.GetButtonDown("Fire1") && GetComponentInChildren<SpinAttackScript>().spinAttackBool == false && GetComponent<ChargePunchScript>().hasPunched == false && GetComponent<ChargePunchScript>().punchDelayBool == false && hasStomped == false && anim.enabled==true && anim.GetCurrentAnimatorStateInfo(0).IsName("Headspring") == false)
        {
            anim.SetTrigger("Stomp Land");
            moveDirection.y -= 22f;
            killerFeetCollider.enabled = true;
            hasStomped = true;
            fallDamageTimer = 0.3f;
            fallDamageEnabled = false;
        }
        else if(IsGrounded() == true && hasStomped == true)
        {
             
            GetComponent<Movement>().enabled = false;
            moveDirection.z = 0;
            moveDirection.x = 0;
            anim.SetFloat("InputMagnitude", 0);
            landingMovementStop = true;
            landingMovementStopTimer = 0.5f;
            killerFeetCollider.enabled = false;
            hasStomped = false;
            GetComponent<HealthAndDamage>().attackInvincibility = false;
            
        }

        if(hasStomped == true)
        {
            
           
            GetComponent<HealthAndDamage>().attackInvincibility = true;
            fallDamageTimer -= Time.deltaTime;
            
        }

        if (fallDamageTimer <= 0)
        {

            fallDamageEnabled = true;
        }

        if (landingMovementStop == true)
        {
            landingMovementStopTimer -= Time.deltaTime;
            if(landingMovementStopTimer < 0 && hasTakenFallDamage == false)
            {
                landingMovementStopTimer = 0.5f;
                GetComponent<Movement>().enabled = true;
                landingMovementStop = false;
               
            }
        }
    }

    
}

