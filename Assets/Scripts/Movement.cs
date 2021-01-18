using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Movement : MonoBehaviour
{

    public float InputX;
    public float InputZ;
    [HideInInspector]
    public Vector3 desiredMoveDirection;

    private bool blockRotationPlayer;
    public float desiredRotationSpeed;
    public Animator anim;
    private float inputSpeed;
    public float runSpeed =1;
    private float allowPlayerRotation;
    private Camera cam;
    private CharacterController controller;
    private float verticalVel;
    private Vector3 moveVector;
    private float diagonalMagnitude;



    //private float cameraDistance;

    void Start()
    {
        anim = this.GetComponent<Animator>();
        cam = Camera.main;
        controller = this.GetComponent<CharacterController>();
        //cameraDistance = GetComponent<CameraCollision>().distance;
    }

    void Update()
    {
        InputMagnitude();

        
    }

    void PlayerMoveAndRotation()
    {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        var forward = cam.transform.forward;
        var right = cam.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        desiredMoveDirection = forward * InputZ + right * InputX;

        if (InputX > 0 && InputZ > 0)
        {
            diagonalMagnitude = (forward.magnitude / 2) * InputZ + (right.magnitude / 2) * InputX;
        }
        else if (InputX < 0 && InputZ > 0)
        {
            diagonalMagnitude = (forward.magnitude / 2) * InputZ - (right.magnitude / 2) * InputX;
        }
        else if (InputX > 0 && InputZ < 0)
        {
            diagonalMagnitude = (right.magnitude / 2) * InputX - (forward.magnitude / 2) * InputZ;
        }
        else if (InputX < 0 && InputZ < 0)
        {
            diagonalMagnitude = (forward.magnitude / 2) * -InputZ - (right.magnitude / 2) * InputX;
        }

        if(GameObject.Find("Spin-Attack").GetComponent<SpinAttackScript>().spinAttackBool == false)
        {
            blockRotationPlayer = false;
        }
        else
        {
            blockRotationPlayer = true;
        }

        //&& GetComponent<Jumper>().hasStomped == false
        if (blockRotationPlayer == false)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
            /*if (GetComponent<Jumper>().hasStomped == true)
            {
                transform.rotation = Quaternion.Euler(0, 0, 90);
            }*/
        }
    }

    void InputMagnitude()
    {
        //Calculate Input Vectors
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        anim.SetFloat("InputZ", InputZ, 0.0f, Time.deltaTime * 2f);
        anim.SetFloat("InputX", InputX, 0.0f, Time.deltaTime * 2f);

        //Calculate the Input Magnitude
        inputSpeed = new Vector2(InputX, InputZ).sqrMagnitude;
        //moveVector = new Vector3((transform.position.x - cam.transform.position.x)/35, 0, (transform.position.z - cam.transform.position.z)/35);

        //Physically move player
        if (inputSpeed > allowPlayerRotation && anim.GetCurrentAnimatorStateInfo(0).IsName("Headspring") == false)
        {
            anim.SetFloat("InputMagnitude", inputSpeed, 0.0f, Time.deltaTime);
            //controller.Move(moveVector);
            PlayerMoveAndRotation();
            //The player was moving faster when moving diagonally, this sets the movement speed to be the same as when moving forward
            if ((InputX>0 && InputZ>0) || (InputX>0 && InputZ<0) || (InputX<0 && InputZ<0) || (InputX<0 && InputZ>0))
            {
                desiredMoveDirection.Normalize();
                desiredMoveDirection *= diagonalMagnitude;
            }
            desiredMoveDirection /= 10;
            controller.Move(desiredMoveDirection*runSpeed);
        } else if (inputSpeed < allowPlayerRotation)
        {
            anim.SetFloat("InputMagnitude", inputSpeed, 0.0f, Time.deltaTime);
            controller.Move(Vector3.zero);
        }
    }
}

