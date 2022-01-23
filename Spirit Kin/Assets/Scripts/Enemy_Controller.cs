using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Controller : MonoBehaviour
{
    //Waypoints (4-8 waypoints around the shrine)
        //more will ensure less obvious patrol paths
            //can generate paths rather than having them set

    //SEPARATE SCRIPT (SHRINE CONTROLLER)
        //Split shrines into 4 quadrants
            //Each quadrant can hold at most 1/2 of total enemies
                //if a quadrant has more than 1/2 of total enemies select enough enemies at random to where its below 1/2
                    //if enemy is not current chasing player change the randomly selected enemies to relocating state
                        //send them to a neighboring quadrant for ease of use

    //Idling enemy would have a smaller detection radius than that of a Patroling one

    //Patroling -> Idling
        //if less than 25% are patroling
            //Increased chance each interval
        //else
            //Standard chance each interval

    //Idling -> Patroling
        //if less than 50% of enemies are patroling
            //Standard chance each interval
        //else
            //Stay idle

    public enum MotionState {
        Patroling,
        Idling,
        Alerted,
        Chasing,
        Returning,
        Relocating
    }

    //Enemies will surround player
        //They will attack once at a time

    public enum AttackState {
        Attacking,
        Surrounding,
        NotAttacking,
    }

    public MotionState EnemyMotion;
    public AttackState EnemyAttack;

    public NavMeshAgent ThisEnemy;
    public NavMeshPath path;
    public GameObject Me;
    public GameObject player;

    public float patrolToIdleChance;
    public float idleToPatrolChance;

    public float swapStateInterval;

    private float myTime;

    // Start is called before the first frame update
    void Start()
    {
        path = new UnityEngine.AI.NavMeshPath();
        player = GameObject.FindWithTag("Player");
        ThisEnemy = Me.GetComponent<UnityEngine.AI.NavMeshAgent>();
        EnemyMotion = MotionState.Idling;
        EnemyAttack = AttackState.NotAttacking;
    }

    // Update is called once per frame
    void Update()
    {
        myTime += Time.deltaTime;
        switch (EnemyMotion)
        {
            case MotionState.Patroling:
                if (myTime > swapStateInterval)
                {
                    //check with Shrine_Controller if need to relocate
                    float temp = Random.Range(0.0f, 1.0f);
                    //ask shrine_controller if less than 25% are patroling
                        //if yes have increased chance
                    if (temp < patrolToIdleChance) //swap states
                    {
                        EnemyMotion = MotionState.Idling;
                        Debug.Log("Now Idle");
                    }
                    myTime = 0;
                }            
                break;
            case MotionState.Idling:
                if (myTime > swapStateInterval)
                {
                    //check with Shrine_Controller if need to relocate
                    float temp = Random.Range(0.0f, 1.0f);
                    if (temp < idleToPatrolChance) //swap states
                    {
                        EnemyMotion = MotionState.Patroling;
                        Debug.Log("Now Patrol");
                    }
                    myTime = 0;
                }
                break;
            case MotionState.Alerted:
                // Tether movement to player's, but reduce our movement speed. Keep turned towards the player. If player approaches for N seconds, Chasing state
                break;
            case MotionState.Chasing:
                ThisEnemy.CalculatePath(player.transform.position, path);
                if(path.status == UnityEngine.AI.NavMeshPathStatus.PathComplete) { // Check if player is in navmesh. Has something to do with the NavMeshPathStatus enum
                    //if(Vector3.distance(player.transform.position, shrine.transform.position)){
                    ThisEnemy.SetDestination(player.transform.position);
                    //}
                }
                else {}
                break;
            case MotionState.Returning:
                break;
            default:
                break;
        }
    }
}
