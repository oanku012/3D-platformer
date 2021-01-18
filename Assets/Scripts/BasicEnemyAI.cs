using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;


public class BasicEnemyAI : MonoBehaviour {

    public Transform player;
    public List<GameObject> DestinationPoints;
    //public float speed;
    //public float alertDistance;
    public float chaseDistance;
    public float attackingDistance;
    //public float turningSpeed = 0f;
    public float remainingDistance;
    public int minTime;
    public int maxTime;

    private Animator anim;
    private Animator playerAnim;
    private Vector3 direction;
    private NavMeshAgent agent;
    private int selectedDestination;

    
    public bool iDamagedThePlayer;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
        player = GameObject.Find("Robot Kyle").transform;
        playerAnim = GameObject.Find("Robot Kyle").GetComponent<Animator>();
    }
	
   

	// Update is called once per frame
	void Update () {
		
       

        //NPC goes idle if a certain distance away from destination
        if(agent.enabled == true && agent.remainingDistance < remainingDistance && anim.GetCurrentAnimatorStateInfo(0).IsName("sound") == false)
        {
            agent.enabled = false;
            anim.SetBool("isWalking", false);
            anim.SetBool("isIdle", true);
            StartCoroutine(RandomMovement());
        }

        //Alert
        //Vector3.Distance(player.position, transform.position) < alertDistance &&
        //Vector3.Distance(player.position, transform.position) > walkingDistance && ignorePlayer == false
        if (iDamagedThePlayer == true && GameObject.Find("Robot Kyle").GetComponent<HealthAndDamage>().Health > 0)
        {
            agent.enabled = false;
            anim.SetBool("isAlert", true);
            anim.SetBool("isIdle", false);
            anim.SetBool("isRunning", false);
            anim.SetBool("isAttacking", false);
            anim.SetBool("isWalking", false);
            GetComponent<NavMeshAgent>().speed = 2;
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("sound"))
            {
                iDamagedThePlayer = false;
            }
            StopCoroutine(RandomMovement());

        }
        //Attacking
        else if (Vector3.Distance (player.position, transform.position) <= chaseDistance && iDamagedThePlayer == false && GameObject.Find("Robot Kyle").GetComponent<HealthAndDamage>().Health >0 && playerAnim.enabled == true && playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Headspring") == false)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("sound") == false)
            {
                agent.enabled = true;

                /* direction = player.position - transform.position;
                 direction.y = 0;

                 transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation (direction), turningSpeed * Time.deltaTime);

                 transform.Translate(0, 0, speed);*/

                agent.speed = 10;

                agent.SetDestination(player.transform.position);

                anim.SetBool("isRunning", true);
                anim.SetBool("isAttacking", false);
                anim.SetBool("isAlert", false);
                anim.SetBool("isIdle", false);
                anim.SetBool("isWalking", false);



                if (Vector3.Distance(player.position, transform.position) <= attackingDistance)
                {
                    //Adds an attack animation when tiger is close, it looked silly, because the tiger's legs weren't moving so I commented it out
                    /*anim.SetBool("isAttacking", true);
                    anim.SetBool("isRunning", false);
                    anim.SetBool("isAlert", false);
                    anim.SetBool("isIdle", false);
                    anim.SetBool("isWalking", false);*/
                }
            }

        }
        //Idle
        else if (agent.enabled == false && anim.GetBool ("isIdle") == false && iDamagedThePlayer == false && anim.GetCurrentAnimatorStateInfo(0).IsName("sound") == false)
        {
            anim.SetBool("isAttacking", false);
            anim.SetBool("isAlert", false);
            anim.SetBool("isIdle", true);
            anim.SetBool("isRunning", false);
            anim.SetBool("isWalking", false);

            GetComponent<NavMeshAgent>().speed = 2;

            
            StartCoroutine(RandomMovement());
        }

        /*if (ignorePlayer == true && GameObject.Find("Robot Kyle").GetComponent<HealthAndDamage>().invincibilityFrames == false)
        {
            ignorePlayer = false;
            iDamagedThePlayer = false;

        }*/

    }

    public IEnumerator RandomMovement()
    {
        
            GetComponent<NavMeshAgent>().speed = 2;
            int index = Random.Range(minTime, maxTime);
            //Debug.Log("RandomTime: " + index);

            yield return new WaitForSeconds(index);

            int index2 = Random.Range(1, 3);

            switch (index2)
            {
                case 1:
                    //Keeps being idle and calls random movement again
                    Debug.Log("KeepIdle...");
                    StartCoroutine(RandomMovement());
                    break;

                case 2:
                    //Picks a random destination and moves
                    Debug.Log("Move...");
                    agent.enabled = true;
                    int lastDestination = selectedDestination;
                    selectedDestination = Random.Range(0, DestinationPoints.Count);
                    while (selectedDestination == lastDestination || DestinationPoints[selectedDestination].GetComponent<DestinationPointScript>().isUsed == true)
                    {
                        selectedDestination = Random.Range(0, DestinationPoints.Count);
                    }

                    DestinationPoints[lastDestination].GetComponent<DestinationPointScript>().isUsed = false;
                    agent.SetDestination(DestinationPoints[selectedDestination].transform.position);
                    DestinationPoints[selectedDestination].GetComponent<DestinationPointScript>().isUsed = true;

                    anim.SetBool("isAttacking", false);
                    anim.SetBool("isAlert", false);
                    anim.SetBool("isIdle", false);
                    anim.SetBool("isRunning", false);
                    anim.SetBool("isWalking", true);
                    break;
            }
        
    }
}
